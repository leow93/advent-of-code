package main

import (
	"fmt"
	"strconv"
	"strings"
	"sync"

	"github.com/leow93/advent-of-code/2019/intcode"
	"github.com/leow93/advent-of-code/utils"
)

func parse(lines []string) []int64 {
	// just need first line
	line := lines[0]
	parts := strings.Split(line, ",")
	result := make([]int64, len(parts))

	for i, p := range parts {
		x, err := strconv.Atoi(p)
		if err != nil {
			panic(err)
		}
		result[i] = int64(x)
	}
	return result
}

type direction int

const (
	up direction = iota
	down
	left
	right
)

type point struct {
	x, y int
}
type robot struct {
	mx  sync.Mutex
	pos *point
	d   direction
}

func (r *robot) position() point {
	return *r.pos
}

func (r *robot) dir() direction {
	return r.d
}

func (r *robot) turnLeft() {
	switch r.d {
	case up:
		r.d = left
	case down:
		r.d = right
	case left:
		r.d = down
	case right:
		r.d = up
	}
}

func (r *robot) turnRight() {
	switch r.d {
	case up:
		r.d = right
	case down:
		r.d = left
	case left:
		r.d = up
	case right:
		r.d = down
	}
}

func (r *robot) moveForward() {
	switch r.d {
	case up:
		r.pos.x--
	case down:
		r.pos.x++
	case left:
		r.pos.y--
	case right:
		r.pos.y++
	}
}

type panels struct {
	data map[point]int
	mx   sync.Mutex
}

func (p *panels) add(x point, colour int) {
	p.data[x] = colour
}

func (p *panels) get(x point) int {
	return p.data[x]
}

func (p *panels) len() int {
	return len(p.data)
}

func directionToArrow(d direction) rune {
	switch d {
	case up:
		return '^'
	case down:
		return 'v'
	case left:
		return '<'
	case right:
		return '>'
	default:
		return 'o'
	}
}

func printGrid(p *panels, robot *robot) {
	size := 80

	rows := make([][]rune, size)
	for i := range rows {
		row := make([]rune, size)
		for j := range row {
			row[j] = '.'
		}
		rows[i] = row
	}

	for k, c := range p.data {
		i := k.x + size/2
		j := k.y + size/2

		r := '.'
		if c == 1 {
			r = '#'
		}
		rows[i][j] = r
	}

	// paint the robot
	pos := robot.position()
	i := pos.x + size/2
	j := pos.y + size/2
	rows[i][j] = directionToArrow(robot.dir())
	for _, row := range rows {
		fmt.Println(string(row))
	}
}

func runProgram(p []int64, robot *robot, panels *panels, start point, startColour int) {
	inReady := make(chan struct{})
	in := make(chan int64)
	out := make(chan int64)

	executionDone := make(chan struct{})
	outputDone := make(chan struct{})

	panels.add(start, startColour)

	go func() {
		defer func() {
			close(in)
			close(inReady)
			close(out)
			executionDone <- struct{}{}
		}()

		mem := intcode.NewMemory(p)
		intcode.RunProgram(mem, inReady, in, out)
	}()

	go func() {
		defer func() {
			outputDone <- struct{}{}
		}()
		i := 0
		for {
			select {
			case _, ok := <-inReady:
				if !ok {
					return
				}
				pos := robot.position()
				in <- int64(panels.get(pos))
			case x, ok := <-out:
				if !ok {
					return
				}
				if i%2 == 0 {
					// paint
					p := robot.position()
					panels.add(p, int(x))
				} else {
					// turn
					if x == 0 {
						robot.turnLeft()
					} else {
						robot.turnRight()
					}
					robot.moveForward()
				}
				i++

			}
		}
	}()

	<-executionDone
	<-outputDone
}

func partOne(p []int64) *panels {
	robot := &robot{
		d:   up,
		pos: &point{0, 0},
		mx:  sync.Mutex{},
	}
	panels := &panels{make(map[point]int), sync.Mutex{}}

	runProgram(p, robot, panels, point{0, 0}, 0)
	return panels
}

func partTwo(p []int64, origPanels *panels) {
	var start *point
	for k := range origPanels.data {
		start = &k
		break
	}
	if start == nil {
		panic("expected start not to be nil")
	}

	robot := &robot{
		d:   up,
		pos: &point{start.x, start.y},
		mx:  sync.Mutex{},
	}
	ps := &panels{make(map[point]int), sync.Mutex{}}
	runProgram(p, robot, ps, *start, 1)

	printGrid(ps, robot)
}

func main() {
	lines := utils.ReadLines()
	p := parse(lines)
	panels := partOne(p)
	fmt.Println("Part I", panels.len())
	fmt.Println("Part II:")
	partTwo(p, panels)
}
