package two

import (
	"bufio"
	"context"
	"encoding/json"
	"fmt"
	"io"
	"os"
	"os/signal"
	"sync"
	"syscall"
	"time"

	"github.com/leow93/advent-of-code/2019/intcode"
	"golang.org/x/term"
)

func idToRune(id int) rune {
	switch id {
	case 1:
		return '|'
	case 2:
		return 'x'
	case 3:
		return '-'
	case 4:
		return 'o'
	default:
		return ' '
	}
}

type state struct {
	grid  [][]rune
	score int
	mx    sync.Mutex
}

func (s *state) setCoord(y, x int, c rune) {
	s.mx.Lock()
	defer s.mx.Unlock()
	s.grid[y][x] = c
}

func (s *state) setScore(score int) {
	s.mx.Lock()
	defer s.mx.Unlock()
	s.score = score
}

func render(state *state) {
	fmt.Print(CursorHome)
	for _, line := range state.grid {
		for j := range line {
			fmt.Printf("%c", line[j])
		}
		fmt.Printf("%s", "\r\n")
	}

	fmt.Printf("Score: %d", state.score)
	fmt.Printf("%s", "\r\n")
}

const (
	CursorHome = "\033[H" // Move cursor to top-left
	HideCursor = "\033[?25l"
	ShowCursor = "\033[?25h"
)

func run(mem *intcode.Memory, inReady chan struct{}, in chan int64, out chan int64, cleanup func()) {
	defer cleanup()
	err := intcode.RunProgram(mem, inReady, in, out)
	if err != nil {
		panic(err)
	}
}

func readInput(tty *os.File, grid [][]rune, input chan<- int, quit chan<- struct{}, moves *[]int) {
	reader := bufio.NewReader(tty)

	if moves != nil && len(*moves) > 0 {
		for _, m := range *moves {
			input <- m
		}
	}

	for {
		b, err := reader.ReadByte()
		if err != nil {
			continue
		}

		switch b {
		case 'a':
			*moves = append(*moves, -1)
			input <- -1 // left
		case 's':
			*moves = append(*moves, 0)
			input <- 0
		case 'd':
			*moves = append(*moves, 1)
			input <- 1 // right
		case 'q', 27:
			close(quit)
			return
		}

	}
}

func selectInput(input <-chan int) int64 {
	x := <-input
	return int64(x)
}

func loop(ctx context.Context, state *state, ticker <-chan time.Time, inReady <-chan struct{}, input <-chan int, in chan<- int64, out <-chan int64) {
	parts := make([]int, 0, 3)
	for {
		select {
		case _, ok := <-inReady:
			if !ok {
				return
			}

			go func() {
				in <- selectInput(input)
			}()

		case x, ok := <-out:
			if !ok {
				return
			}
			parts = append(parts, int(x))
			if len(parts) == 3 {
				x, y, id := parts[0], parts[1], parts[2]
				if x == -1 && y == 0 {
					state.setScore(id)
				} else {
					state.setCoord(y, x, idToRune(id))
				}

				parts = make([]int, 0, 3)
			}
		case <-ctx.Done():
			return
		case <-ticker:
			render(state)
		}
	}
}

func enableRawMode(fd int) func() {
	if !term.IsTerminal(fd) {
		return func() {}
	}

	oldState, err := term.MakeRaw(fd)
	if err != nil {
		panic(err)
	}

	return func() {
		term.Restore(fd, oldState)
	}
}

func readMoves() []int {
	var moves []int
	f, err := os.Open("moves.json")
	if err != nil {
		return moves
	}

	defer f.Close()

	bs, err := io.ReadAll(f)
	if err != nil {
		panic(err)
	}

	err = json.Unmarshal(bs, &moves)
	if err != nil {
		panic(err)
	}

	return moves
}

func Run(p []int64) {
	p[0] = 2
	tty, err := os.Open("/dev/tty")
	if err != nil {
		panic(err)
	}
	defer tty.Close()

	restore := enableRawMode(int(tty.Fd()))
	defer restore()

	// Hide cursor
	fmt.Print(HideCursor)
	defer fmt.Print(ShowCursor)

	// Clear screen and move cursor home
	fmt.Print("\033[2J\033[H")

	quit := make(chan struct{})
	sig := make(chan os.Signal, 1)
	signal.Notify(sig, syscall.SIGINT, syscall.SIGTERM)
	go func() {
		<-sig
		close(quit)
	}()

	size := 50
	grid := make([][]rune, size)
	for i := range size {
		row := make([]rune, size)
		for j := range size {
			row[j] = ' '
		}
		grid[i] = row
	}
	state := &state{
		grid:  grid,
		score: 0,
		mx:    sync.Mutex{},
	}

	moves := readMoves()

	input := make(chan int, 10)
	go readInput(tty, grid, input, quit, &moves)

	ticker := time.NewTicker(50 * time.Millisecond)
	defer ticker.Stop()
	mem := intcode.NewMemory(p)
	in := make(chan int64)
	inReady := make(chan struct{})
	out := make(chan int64)

	loopDone := make(chan struct{})

	ctx, cancel := context.WithCancel(context.Background())
	go func() {
		loop(ctx, state, ticker.C, inReady, input, in, out)
		loopDone <- struct{}{}
	}()
	go run(mem, inReady, in, out, func() {
		close(in)
		close(inReady)
		close(out)
		cancel()
		quit <- struct{}{}
	})

	<-quit
	<-loopDone

	bs, _ := json.Marshal(moves)
	os.WriteFile("moves.json", bs, 0644)
	// render one last time
	render(state)
}
