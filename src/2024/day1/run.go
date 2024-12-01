package day1

import (
	"fmt"
	"slices"
	"strconv"
	"strings"

	"github.com/leow93/advent-of-code/2024/input"
)

func getLeftRight(lines []string) ([]int, []int, error) {
	var left, right []int

	for _, line := range lines {
		parts := strings.Fields(line)
		if len(parts) != 2 {
			continue
		}
		l, err := strconv.Atoi(parts[0])
		if err != nil {
			return nil, nil, err
		}
		r, err := strconv.Atoi(parts[1])
		if err != nil {
			return nil, nil, err
		}
		left = append(left, l)
		right = append(right, r)
	}

	return left, right, nil
}

func distance(a, b int) int {
	if a > b {
		return a - b
	}
	return b - a
}

func cmp(a, b int) int {
	if a < b {
		return -1
	}
	if a > b {
		return 1
	}
	return 0
}

func partOne(l []int, r []int) (string, error) {
	left := slices.Clone(l)
	right := slices.Clone(r)

	slices.SortFunc(left, cmp)
	slices.SortFunc(right, cmp)

	result := 0
	for i := range len(left) {
		l := left[i]
		r := right[i]
		result += distance(l, r)
	}

	return fmt.Sprintf("%d", result), nil
}

func count(xs []int, test int) int {
	result := 0
	for _, x := range xs {
		if x == test {
			result += 1
		}
	}
	return result
}

func partTwo(left []int, right []int) (string, error) {
	result := 0
	for _, x := range left {
		c := count(right, x)
		result += x * c
	}

	return fmt.Sprintf("%d", result), nil
}

func Run(file string) (string, string, error) {
	lines, err := input.ReadLines(file)
	if err != nil {
		return "", "", err
	}
	left, right, err := getLeftRight(lines)
	if err != nil {
		return "", "", err
	}

	one, err := partOne(left, right)
	if err != nil {
		return "", "", err
	}
	two, err := partTwo(left, right)
	if err != nil {
		return "", "", err
	}
	return one, two, nil
}
