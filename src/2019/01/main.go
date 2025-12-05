package main

import (
	"fmt"
	"io"
	"math"
	"os"
	"strconv"
)

func readLines() []string {
	data, err := io.ReadAll(os.Stdin)
	if err != nil {
		panic(err)
	}
	result := make([]string, 0)

	buf := ""
	for _, b := range data {
		if b == '\n' {
			result = append(result, buf)
			buf = ""
		} else {
			buf += string(b)
		}
	}

	return result
}

func fuelForMass(mass int) int {
	f := float64(mass)
	f = math.Floor(f/3) - 2
	return int(f)
}

func fuelRequiredRecursive(mass int) int {
	f := float64(mass)
	f = math.Floor(f/3) - 2

	result := int(f)
	if result <= 0 {
		return 0
	}
	return result + fuelRequiredRecursive(result)
}

func run(fuelF func(mass int) int) func(input []string) int {
	return func(input []string) int {
		result := 0
		for _, l := range input {
			mass, err := strconv.Atoi(l)
			if err != nil {
				panic(err)
			}
			result += fuelF(mass)

		}
		return result
	}
}

func main() {
	lines := readLines()

	partOne := run(fuelForMass)
	partTwo := run(fuelRequiredRecursive)

	fmt.Println("Part 1", partOne(lines))
	fmt.Println("Part 2", partTwo(lines))
}
