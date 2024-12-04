package main

import (
	"errors"
	"flag"
	"fmt"

	"github.com/leow93/advent-of-code/2024/day1"
	"github.com/leow93/advent-of-code/2024/day2"
	"github.com/leow93/advent-of-code/2024/day3"
	"github.com/leow93/advent-of-code/2024/day4"
	"github.com/leow93/advent-of-code/2024/input"
)

type Runner func(input []string) (string, string, error)

var days = map[string]Runner{
	"day1": day1.Run,
	"day2": day2.Run,
	"day3": day3.Run,
	"day4": day4.Run,
}

func main() {
	flag.Parse()
	day := flag.Arg(0)
	runner, ok := days[day]
	if !ok {
		panic(errors.New("day " + day + " not found"))
	}

	data, err := input.FromStdin()
	if err != nil {
		panic(err)
	}
	part1, part2, err := runner(data)
	if err != nil {
		panic(err)
	}

	fmt.Printf("Part I: %s\n", part1)
	fmt.Printf("Part II: %s\n", part2)
}
