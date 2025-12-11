package main

import (
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

func main() {
	lines := utils.ReadLines()
	p := parse(lines)
	input := make(chan int)
	done := make(chan struct{})
	go func() {
		err := intcode.RunProgram(p, input)
		if err != nil {
			panic(err)
		}
		done <- struct{}{}
	}()
	go func() {
		input <- 1
	}()

	<-done
}
