package main

import (
	"errors"
	"fmt"
	"slices"
	"strconv"
	"strings"

	"github.com/leow93/advent-of-code/2024/input"
)

type rule struct {
	x int
	y int
}

type update struct {
	pages []int
}

func (u *update) valid(rules []rule) bool {
	for i := 0; i < len(u.pages); i++ {
		for j := i + 1; j < len(u.pages); j++ {
			x := u.pages[i]
			y := u.pages[j]
			for _, r := range rules {
				if wrongOrder := r.x == y && r.y == x; wrongOrder {
					return false
				}
			}
		}
	}

	return true
}

func parseRule(r string) (rule, error) {
	parts := strings.Split(r, "|")
	if len(parts) != 2 {
		return rule{}, errors.New("invalid rule: " + r)
	}

	x, err := strconv.Atoi(parts[0])
	if err != nil {
		return rule{}, err
	}
	y, err := strconv.Atoi(parts[1])
	if err != nil {
		return rule{}, err
	}

	return rule{x, y}, nil
}

func parseUpdate(u string) (update, error) {
	parts := strings.Split(u, ",")
	var pages []int
	for _, p := range parts {
		x, err := strconv.Atoi(p)
		if err != nil {
			return update{}, err
		}

		pages = append(pages, x)
	}

	return update{pages}, nil
}

func parse(input []string) ([]rule, []update, error) {
	parsingRules := true
	var rules []rule
	var updates []update

	for _, line := range input {
		if line == "" {
			parsingRules = false
			continue
		}

		if parsingRules {
			r, err := parseRule(line)
			if err != nil {
				return nil, nil, err
			}
			rules = append(rules, r)
		} else {
			u, err := parseUpdate(line)
			if err != nil {
				return nil, nil, err
			}
			updates = append(updates, u)
		}
	}

	return rules, updates, nil
}

func partOne(rs []rule, us []update) string {
	sum := 0
	for _, u := range us {
		if u.valid(rs) {
			sum += u.pages[len(u.pages)/2]
		}
	}

	return fmt.Sprintf("%d", sum)
}

func sortPages(rs []rule) func(x int, y int) int {
	return func(x, y int) int {
		for _, rule := range rs {
			if rule.x == x && rule.y == y {
				return -1
			}
			if rule.x == y && rule.y == x {
				return 1
			}
		}
		return 0
	}
}

func fixInvalid(rs []rule, us []update) []update {
	var result []update

	cmp := sortPages(rs)

	for _, u := range us {
		pages := slices.Clone(u.pages)
		slices.SortFunc(pages, cmp)
		result = append(result, update{pages})
	}
	return result
}

func partTwo(rs []rule, us []update) string {
	var invalid []update
	for _, u := range us {
		if !u.valid(rs) {
			invalid = append(invalid, u)
		}
	}

	valid := fixInvalid(rs, invalid)

	sum := 0
	for _, u := range valid {
		if u.valid(rs) {
			sum += u.pages[len(u.pages)/2]
		}
	}

	return fmt.Sprintf("%d", sum)
}

func Run(input []string) (string, string, error) {
	rs, us, err := parse(input)
	if err != nil {
		return "", "", err
	}
	one := partOne(rs, us)
	two := partTwo(rs, us)

	return one, two, nil
}

func main() {
	i, err := input.FromStdin()
	if err != nil {
		panic(err)
	}

	one, two, err := Run(i)
	if err != nil {
		panic(err)
	}

	fmt.Printf("Part I: %s\n", one)
	fmt.Printf("Part II: %s\n", two)
}
