package main

import (
	"fmt"
	"strconv"
	"strings"
	"sync"

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

func run(program []int, in int) int {
	p := make([]int, len(program))
	copy(p, program)

	var result int
	input := make(chan int)
	output := make(chan int)
	done := make(chan struct{})

	ioWg := sync.WaitGroup{}
	ioWg.Add(2)

	go func() {
		err := intcode.RunProgram(p, input, output)
		if err != nil {
			panic(err)
		}
		done <- struct{}{}
	}()
	go func() {
		defer ioWg.Done()
		input <- in
	}()

	go func() {
		defer ioWg.Done()
		for x := range output {
			result = x
			fmt.Println(result)
		}
	}()
	<-done
	close(input)
	close(output)

	ioWg.Wait()
	return result
}

func main() {
	lines := utils.ReadLines()
	p := parse(lines)
	p1 := run(p, 1)
	fmt.Println("Part I", p1)
	p2 := run(p, 5)
	fmt.Println("Part II", p2)
}
