package main

import (
	"fmt"
	"strconv"
	"strings"

	"github.com/leow93/advent-of-code/2019/13/two"
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

func partOne(p []int64) int {
	executionDone := make(chan struct{})
	outputDone := make(chan struct{})

	mem := intcode.NewMemory(p)
	blocks := 0

	out := make(chan int64)
	go func() {
		defer func() {
			close(out)
			executionDone <- struct{}{}
		}()
		err := intcode.RunProgram(mem, nil, nil, out)
		if err != nil {
			panic(err)
		}
	}()

	go func() {
		defer func() {
			outputDone <- struct{}{}
		}()

		parts := make([]int, 0, 3)
		for x := range out {
			parts = append(parts, int(x))
			if len(parts) == 3 {
				id := parts[2]
				if id == 2 {
					blocks++
				}
				parts = make([]int, 0, 3)
			}
		}
	}()

	<-executionDone
	<-outputDone

	return blocks
}

func main() {
	lines := utils.ReadLines()
	p := parse(lines)
	fmt.Println("Part I: ", partOne(p))
	fmt.Println("Part II: ", two.Run(p))
}
