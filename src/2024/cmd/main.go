package main

import (
	"errors"
	"flag"
	"fmt"
	"strings"

	"github.com/leow93/advent-of-code/2024/day1"
)

type Runner func(file string) (string, string, error)

var days = map[string]Runner{
	"day1": day1.Run,
}

func pathToFile(day, file string) string {
	return fmt.Sprintf("./%s/%s", day, file)
}

func dayFromInput(file string) (string, error) {
	parts := strings.Split(file, "/")
	var day string
	for _, p := range parts {
		if strings.HasPrefix(p, "day") {
			day = p
			break
		}
	}
	if day == "" {
		return "", errors.New("day not found")
	}
	return day, nil
}

func main() {
	flag.Parse()
	inputFile := flag.Arg(0)
	day, err := dayFromInput(inputFile)
	if err != nil {
		panic(err)
	}
	runner, ok := days[day]
	if !ok {
		panic(errors.New("day " + day + " not found"))
	}

	part1, part2, err := runner(inputFile)
	if err != nil {
		panic(err)
	}

	fmt.Printf("Part I: %s\n", part1)
	fmt.Printf("Part II: %s\n", part2)
}
