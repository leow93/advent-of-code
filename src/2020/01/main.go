package main

import (
	"bufio"
	"fmt"
	"os"
	"strconv"
)

func fromStdin() ([]string, error) {
	var data []string
	scanner := bufio.NewScanner(os.Stdin)

	for scanner.Scan() {
		data = append(data, scanner.Text())
	}
	return data, scanner.Err()
}

func parse(s []string) ([]int, error) {
	var result []int
	for _, x := range s {
		n, err := strconv.Atoi(x)
		if err != nil {
			return nil, err
		}
		result = append(result, n)
	}
	return result, nil
}

func one(xs []int) int {
	for i := 0; i < len(xs)-1; i++ {
		for j := i + 1; j < len(xs); j++ {
			x := xs[i]
			y := xs[j]
			if x+y == 2020 {
				return x * y
			}
		}
	}

	return 0
}

func two(xs []int) int {
	for i := 0; i < len(xs)-2; i++ {
		for j := i + 1; j < len(xs)-1; j++ {
			for k := j + 1; k < len(xs); k++ {

				x := xs[i]
				y := xs[j]
				z := xs[k]
				if x+y+z == 2020 {
					return x * y * z
				}

			}
		}
	}

	return 0
}

func main() {
	data, err := fromStdin()
	if err != nil {
		panic(err)
	}
	xs, err := parse(data)
	if err != nil {
		panic(err)
	}

	x := one(xs)
	fmt.Printf("Part I: %d\n", x)

	x = two(xs)
	fmt.Printf("Part II: %d\n", x)
}
