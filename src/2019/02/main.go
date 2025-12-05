package main

import (
	"fmt"
	"strconv"
	"strings"

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

func runProgram(program []int) int {
	i := 0

	for i < len(program)-3 {
		opcode := program[i]
		if opcode == 99 {
			return program[0]
		}
		a := program[i+1]
		b := program[i+2]
		idx := program[i+3]
		if opcode == 1 {
			program[idx] = program[a] + program[b]
		}
		if opcode == 2 {
			program[idx] = program[a] * program[b]
		}

		i += 4
	}
	return -1
}

func partOne(program []int) int {
	p := make([]int, len(program))
	copy(p, program)

	p[1] = 12
	p[2] = 2

	return runProgram(p)
}

func partTwo(program []int) int {
	target := 19690720

	for i := range 100 {
		for j := range 100 {
			p := make([]int, len(program))
			copy(p, program)
			p[1] = i
			p[2] = j

			if result := runProgram(p); result == target {
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
