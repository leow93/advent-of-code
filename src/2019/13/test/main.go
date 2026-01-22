package main

import (
	"bufio"
	"fmt"
	"os"
	"os/signal"
	"syscall"
	"time"

	"golang.org/x/term"
)

const (
	fps    = 60
	width  = 30
	height = 12
)

// ---------- Terminal helpers ----------

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

// ---------- Input ----------

func readInput(tty *os.File, move chan<- [2]int, quit chan<- struct{}) {
	reader := bufio.NewReader(tty)

	for {
		b, err := reader.ReadByte()
		if err != nil {
			continue
		}

		switch b {
		case 'q', 27:
			close(quit)
			return
		case 'w':
			move <- [2]int{0, -1}
		case 's':
			move <- [2]int{0, 1}
		case 'a':
			move <- [2]int{-1, 0}
		case 'd':
			move <- [2]int{1, 0}
		}
	}
}

// ---------- Grid ----------

func newGrid() [][]rune {
	grid := make([][]rune, height)
	for y := 0; y < height; y++ {
		grid[y] = make([]rune, width)
		for x := 0; x < width; x++ {
			grid[y][x] = ' '
		}
	}
	return grid
}

func clearGrid(grid [][]rune) {
	for y := 0; y < height; y++ {
		for x := 0; x < width; x++ {
			grid[y][x] = ' '
		}
	}
}

// ---------- Rendering ----------

func render(grid [][]rune) {
	fmt.Print("\033[H") // cursor home

	// Top border
	fmt.Print("+")
	for i := 0; i < width; i++ {
		fmt.Print("-")
	}
	fmt.Print("+\r\n")

	// Rows
	for y := 0; y < height; y++ {
		fmt.Print("|")
		for x := 0; x < width; x++ {
			fmt.Print(string(grid[y][x]))
		}
		fmt.Print("|\r\n")
	}

	// Bottom border
	fmt.Print("+")
	for i := 0; i < width; i++ {
		fmt.Print("-")
	}
	fmt.Print("+\r\n")
}

// ---------- Main ----------

func main() {
	fmt.Print("Press ENTER to start the game...")
	fmt.Scanln()

	tty, err := os.Open("/dev/tty")
	if err != nil {
		fmt.Fprintln(os.Stderr, "No controlling terminal available")
		os.Exit(1)
	}
	defer tty.Close()

	restore := enableRawMode(int(tty.Fd()))
	defer restore()

	// Hide cursor
	fmt.Print("\033[?25l")
	defer fmt.Print("\033[?25h")

	// Clear screen
	fmt.Print("\033[2J")

	quit := make(chan struct{})
	move := make(chan [2]int, 10)

	// Signals
	sig := make(chan os.Signal, 1)
	signal.Notify(sig, syscall.SIGINT, syscall.SIGTERM)
	go func() {
		<-sig
		close(quit)
	}()

	go readInput(tty, move, quit)

	grid := newGrid()
	px, py := width/2, height/2

	ticker := time.NewTicker(time.Second / fps)
	defer ticker.Stop()

	for {
		select {
		case <-quit:
			fmt.Print("\033[H")
			fmt.Println("Goodbye!")
			return

		case d := <-move:
			nx := px + d[0]
			ny := py + d[1]

			if nx >= 0 && nx < width {
				px = nx
			}
			if ny >= 0 && ny < height {
				py = ny
			}

		case <-ticker.C:
			clearGrid(grid)
			grid[py][px] = '@'
			render(grid)
		}
	}
}
