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

func run(p []int64, input int64) int64 {
	var result int64
	inReady := make(chan struct{})
	in := make(chan int64)
	out := make(chan int64)
	done := make(chan struct{})
	mem := intcode.NewMemory(p)

	go func() {
		<-inReady
		in <- input
	}()

	go func() {
		defer func() {
			close(out)
			done <- struct{}{}
		}()
		err := intcode.RunProgram(mem, inReady, in, out)
		if err != nil {
			panic(err)
		}
	}()

	go func() {
		for x := range out {
			result = x
		}
		done <- struct{}{}
	}()

	<-done
	<-done

	return result
}

func main() {
	lines := utils.ReadLines()
	p := parse(lines)
	fmt.Println("Part I", run(p, 1))
	fmt.Println("Part II", run(p, 2))
}
