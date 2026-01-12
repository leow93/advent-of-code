package main

import (
	"fmt"

	"github.com/leow93/advent-of-code/utils"
)

func parse(lines []string) []int {
	line := lines[0]
	result := make([]int, len(line))

	for i, c := range line {
		result[i] = int(c - '0')
	}
	return result
}

func resolvePatternElm(repeat int, i int) int {
	base := []int{0, 1, 0, -1}
	j := ((1 + i) / repeat) % 4
	return base[j]
}

func abs(x int) int {
	if x < 0 {
		return -x
	}
	return x
}

func ones(x int) int {
	if abs(x) < 10 {
		return abs(x)
	}

	return abs(x % 10)
}

func fft(input []int, phases int) []int {
	data := make([]int, len(input))
	copy(data, input)

	for range phases {
		curr := make([]int, len(data))
		copy(curr, data)

		for i := range data {
			total := 0

			for j, x := range data {
				rpe := resolvePatternElm(i+1, j)
				y := x * rpe
				total += y
			}

			curr[i] = ones(total)
		}

		data = curr
	}
	return data
}

func join(xs []int) string {
	result := ""
	for i := range xs {
		result += fmt.Sprintf("%d", xs[i])
	}
	return result
}

func partOne(input []int) string {
	return join(fft(input, 100)[0:8])
}

func main() {
	lines := utils.ReadLines()
	input := parse(lines)
	p1 := partOne(input)
	if p1 != "10189359" {
		fmt.Printf("got %s, want %s\n", p1, "10189359")
		return
	}
	fmt.Print("Part I:", partOne(input))
}
