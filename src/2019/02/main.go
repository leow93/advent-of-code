package main

import (
	"fmt"
	"strconv"
	"strings"

	"github.com/leow93/advent-of-code/2019/intcode"
	"github.com/leow93/advent-of-code/utils"
)

func parse(lines []string) []int {
	// just need first line
	line := lines[0]
	parts := strings.Split(line, ",")
	result := make([]int, len(parts))

	for i, p := range parts {
		x, err := strconv.Atoi(p)
		if err != nil {
			panic(err)
		}
		result[i] = x
	}
	return result
}

func partOne(program []int) int {
	p := make([]int, len(program))
	copy(p, program)

	p[1] = 12
	p[2] = 2

	err := intcode.RunProgram(p, nil, nil)
	if err != nil {
		panic(err)
	}
	return p[0]
}

func partTwo(program []int) int {
	target := 19690720

	for i := range 100 {
		for j := range 100 {
			p := make([]int, len(program))
			copy(p, program)
			p[1] = i
			p[2] = j
			err := intcode.RunProgram(p, nil, nil)
			if err != nil {
				panic(err)
			}
			if p[0] == target {
				return (100 * i) + j
			}
		}
	}
	return -1
}

func main() {
	lines := utils.ReadLines()
	p := parse(lines)
	fmt.Println("Part 1", partOne(p))
	fmt.Println("Part 2", partTwo(p))
}
