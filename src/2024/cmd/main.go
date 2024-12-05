package main

import (
	"errors"
	"flag"
	"fmt"
	"os"

	"github.com/leow93/advent-of-code/2024/day1"
	"github.com/leow93/advent-of-code/2024/day2"
	"github.com/leow93/advent-of-code/2024/day3"
	"github.com/leow93/advent-of-code/2024/day4"
	"github.com/leow93/advent-of-code/2024/day5"
	"github.com/leow93/advent-of-code/2024/input"
)

type Runner func(input []string) (string, string, error)

var ds = []string{"day1", "day2", "day3", "day4", "day5"}

var days = map[string]Runner{
	ds[0]: day1.Run,
	ds[1]: day2.Run,
	ds[2]: day3.Run,
	ds[3]: day4.Run,
	ds[4]: day5.Run,
}

func readFileInput(day string, test bool) ([]string, error) {
	if test {
		return input.ReadLines(fmt.Sprintf("./%s/test_input.txt", day))
	}
	return input.ReadLines(fmt.Sprintf("./%s/input.txt", day))
}

func runDay(day string, test bool) {
	fmt.Printf("%s\n", day)
	defer func() {
		fmt.Printf("\n-------------------\n\n")
	}()
	data, err := readFileInput(day, test)
	if err != nil {
		if errors.Is(err, os.ErrNotExist) {
			fmt.Printf("file does not exist")
			return
		}
		panic(err)
	}
	runner, ok := days[day]
	if !ok {
		panic(errors.New("day " + day + " not found"))
	}

	part1, part2, err := runner(data)
	if err != nil {
		panic(err)
	}

	fmt.Printf("Part I: %s\n", part1)
	fmt.Printf("Part II: %s", part2)
}

func main() {
	day := flag.String("day", "day1", "-day=day4")
	test := flag.Bool("test", false, "-test=true")
	flag.Parse()

	if *day == "all" {
		for _, day := range ds {
			runDay(day, *test)
		}
		return
	}

	runDay(*day, *test)
}
