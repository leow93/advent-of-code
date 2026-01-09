package main

import (
	"fmt"
	"math"
	"strconv"
	"strings"

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

func runComputer(p []int64) (readyForInput chan struct{}, input chan int64, output chan int64) {
	readyForInput = make(chan struct{})
	input = make(chan int64)
	output = make(chan int64)
	mem := intcode.NewMemory(p)

	go func() {
		err := intcode.RunProgram(mem, readyForInput, input, output)
		if err != nil {
			panic(err)
		}
		close(readyForInput)
		close(input)
		close(output)
	}()

	return readyForInput, input, output
}

type coord struct {
	x, y int
}
type move int64

const (
	north move = iota + 1
	south
	west
	east
)

func moveDroid(curr coord, dir move) coord {
	x := curr.x
	y := curr.y
	switch dir {
	case north:
		return coord{x, y - 1}
	case south:
		return coord{x, y + 1}
	case east:
		return coord{x + 1, y}
	case west:
		return coord{x - 1, y}
	}

	panic("unknown move")
}

func opposite(d move) move {
	switch d {
	case north:
		return south
	case south:
		return north
	case west:
		return east
	case east:
		return west
	}

	return south
}

func dfs(
	visited map[coord]bool,
	view map[coord]rune,
	readyForInput chan struct{},
	input chan int64,
	output chan int64,
	curr coord,
	dist int,
	oxygenDist *int,
) {
	for i := 1; i <= 4; i++ {
		dir := move(i)
		next := moveDroid(curr, dir)
		if visited[next] {
			continue
		}

		<-readyForInput
		input <- int64(dir)
		status := <-output

		visited[next] = true
		switch status {
		case 0:
			view[next] = '#'

		case 1, 2:
			if status == 2 && oxygenDist != nil && *oxygenDist == -1 {
				*oxygenDist = dist + 1
				view[next] = 'O'
			} else {
				view[next] = '.'
			}

			dfs(
				visited,
				view,
				readyForInput,
				input,
				output,
				next,
				dist+1,
				oxygenDist,
			)

			// backtrack physically
			<-readyForInput
			input <- int64(opposite(dir))
			<-output
		}

	}
}

func partTwo(view map[coord]rune, start coord) int {
	maxY := 0
	maxX := 0
	minY := math.MaxInt
	minX := math.MaxInt

	for k := range view {
		if k.x > maxX {
			maxX = k.x
		}
		if k.y > maxY {
			maxY = k.y
		}
		if k.x < minX {
			minX = k.x
		}
		if k.y < minY {
			minY = k.y
		}
	}

	q := [][]coord{{start}}
	t := -1
	visited := make(map[coord]bool)

	print := func() {
		fmt.Printf("t=%d\n", t)
		for i := minY; i <= maxY; i++ {
			line := ""
			y := i
			for j := minX; j <= maxX; j++ {
				x := j
				c := coord{x, y}

				r, ok := view[c]
				if !ok {
					r = 'x'
				}
				if visited[c] {
					r = 'O'
				}

				if i == 0 && j == 0 {
					r = 'S'
				}

				line += fmt.Sprintf("%c", r)
			}

			fmt.Println(line)
		}
	}

	for len(q) > 0 {
		cells := q[0]
		q = q[1:]

		print()

		toVisit := make([]coord, 0)
		for _, curr := range cells {

			if visited[curr] {
				continue
			}

			visited[curr] = true

			for i := 1; i <= 4; i++ {
				dir := move(i)
				next := moveDroid(curr, dir)
				if visited[next] {
					continue
				}

				if next.x <= minX || next.x >= maxX || next.y <= minY || next.y >= maxY {
					continue
				}

				if view[next] == '#' {
					continue
				}

				toVisit = append(toVisit, next)
			}
		}

		t += 1
		if len(toVisit) == 0 {
			continue
		}
		q = append(q, toVisit)
	}

	print()

	return t
}

func main() {
	lines := utils.ReadLines()
	p := parse(lines)

	readyForInput, input, output := runComputer(p)
	visited := make(map[coord]bool)
	view := make(map[coord]rune)
	o2Distance := -1
	dfs(visited, view, readyForInput, input, output, coord{0, 0}, 0, &o2Distance)

	var o2 coord
	for k, v := range view {
		if v == 'O' {
			o2 = k
		}
	}

	fmt.Println("Part I: ", o2Distance)
	fmt.Println("Part II: ", partTwo(view, o2))
}
