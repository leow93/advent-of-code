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

func main() {
	lines := utils.ReadLines()
	p := parse(lines)

	c := intcode.NewSeries(p, 1)
	p1 := intcode.RunSeries(c, 1)
	fmt.Println("Part I", p1)
	c = intcode.NewSeries(p, 1)
	p2 := intcode.RunSeries(c, 5)
	fmt.Println("Part II", p2)
}
