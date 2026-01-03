package main

import (
	"fmt"
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

func partOne(program []int64) int64 {
	p := make([]int64, len(program))
	copy(p, program)

	p[1] = 12
	p[2] = 2

	mem := intcode.NewMemory(p)

	err := intcode.RunProgram(mem, nil, nil)
	if err != nil {
		panic(err)
	}
	return mem.Get(0)
}

func partTwo(program []int64) int64 {
	var target int64 = 19690720

	for i := range 100 {
		for j := range 100 {
			p := make([]int64, len(program))
			copy(p, program)
			p[1] = int64(i)
			p[2] = int64(j)
			mem := intcode.NewMemory(p)
			err := intcode.RunProgram(mem, nil, nil)
			if err != nil {
				panic(err)
			}
			if mem.Get(0) == target {
				return int64((100 * i) + j)
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
