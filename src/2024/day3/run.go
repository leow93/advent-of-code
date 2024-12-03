package day3

import (
	"fmt"
	"regexp"
	"strconv"
	"strings"
)

func partOne(input string) (string, error) {
	result := 0
	re := regexp.MustCompile(`mul\((?P<X>\d+),(?P<Y>\d+)\)`)
	for _, match := range re.FindAllStringSubmatch(input, -1) {
		if len(match) < 3 {
			continue
		}
		x, err := strconv.Atoi(match[1])
		if err != nil {
			return "", err
		}
		y, err := strconv.Atoi(match[2])
		if err != nil {
			return "", err
		}

		result += x * y
	}
	return fmt.Sprintf("%d", result), nil
}

func partTwo(input string) (string, error) {
	enabled := true
	result := 0
	re := regexp.MustCompile(`(mul\((?P<X>\d+),(?P<Y>\d+)\))|(do\(\))|(don't\(\))`)

	for _, match := range re.FindAllStringSubmatch(input, -1) {
		if match[0] == "don't()" {
			enabled = false
		} else if match[0] == "do()" {
			enabled = true
		} else if enabled {
			x, err := strconv.Atoi(match[2])
			if err != nil {
				return "", err
			}
			y, err := strconv.Atoi(match[3])
			if err != nil {
				return "", err
			}
			result += x * y
		}
	}
	return fmt.Sprintf("%d", result), nil
}

func Run(input []string) (string, string, error) {
	one, err := partOne(strings.Join(input, ""))
	if err != nil {
		return "", "", err
	}

	two, err := partTwo(strings.Join(input, ""))
	if err != nil {
		return "", "", err
	}

	return one, two, nil
}
