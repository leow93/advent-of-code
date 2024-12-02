package day2

import (
	"fmt"
	"strconv"
	"strings"
)

func asc(a, b int) bool {
	return b > a
}

func distance(a, b int) int {
	if b > a {
		return b - a
	}
	return a - b
}

func distanceOk(a, b int) bool {
	d := distance(a, b)
	return d >= 1 && d <= 3
}

func levelIsSafe(lvl []int) bool {
	if len(lvl) <= 1 {
		return true
	}

	if lvl[0] == lvl[1] {
		return false
	}
	if !distanceOk(lvl[0], lvl[1]) {
		return false
	}

	ascending := asc(lvl[0], lvl[1])

	for i := 1; i < len(lvl)-1; i++ {
		curr := lvl[i]
		next := lvl[i+1]

		_asc := asc(curr, next)
		if ascending != _asc {
			return false
		}

		d := distance(curr, next)
		if d < 1 || d > 3 {
			return false
		}
	}

	return true
}

func partOne(matrix [][]int) string {
	result := 0

	for _, lvl := range matrix {
		if levelIsSafe(lvl) {
			result += 1
		}
	}

	// 269 too high

	return fmt.Sprintf("%d", result)
}

func partTwo() string {
	return ""
}

func parseLines(lines []string) ([][]int, error) {
	var matrix [][]int
	for _, line := range lines {
		parts := strings.Fields(line)
		var ints []int
		for _, p := range parts {
			x, err := strconv.Atoi(p)
			if err != nil {
				return nil, err
			}
			ints = append(ints, x)
		}

		if len(ints) > 0 {
			matrix = append(matrix, ints)
		}
	}

	return matrix, nil
}

func Run(lines []string) (string, string, error) {
	matrix, err := parseLines(lines)
	if err != nil {
		return "", "", err
	}

	return partOne(matrix), partTwo(), nil
}
