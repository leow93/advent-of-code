package day11

import (
	"fmt"
	"strconv"
	"strings"
)

type count struct {
	m map[string]int
}

func newCount() *count {
	return &count{m: make(map[string]int)}
}

func (c *count) inc(key string, amount int) {
	if v, ok := c.m[key]; ok {
		c.m[key] = v + amount
		return
	}
	c.m[key] = amount
}

func (c *count) dec(key string, amount int) {
	if v, ok := c.m[key]; ok {
		next := v - amount
		if next <= 0 {
			delete(c.m, key)
			return
		}
		c.m[key] = next
		return
	}
}

func (c *count) total() int {
	result := 0
	for _, v := range c.m {
		result += v
	}
	return result
}

type entry struct {
	key   string
	value int
}

func (c *count) entries() []entry {
	var result []entry

	for k, v := range c.m {
		result = append(result, entry{k, v})
	}
	return result
}

func scrubLeadingZeroes(s string) string {
	result := s
	for strings.HasPrefix(result, "0") {
		if len(result) == 1 {
			return result
		}
		result = strings.TrimPrefix(result, "0")
	}

	return result
}

func (c *count) blink() {
	for _, e := range c.entries() {
		stone := e.key
		count := e.value
		c.dec(stone, count)
		if stone == "0" {
			c.inc("1", count)
			continue
		}
		if len(stone)&1 == 0 {
			first := stone[:len(stone)/2]
			second := scrubLeadingZeroes(stone[len(stone)/2:])

			c.inc(first, count)
			c.inc(second, count)
			continue
		}

		n, err := strconv.Atoi(stone)
		if err != nil {
			panic(err)
		}
		next := strconv.Itoa(n * 2024)
		c.inc(next, count)
	}
}

func parse(lines []string) *count {
	result := newCount()
	line := lines[0]
	xs := strings.Split(line, " ")
	for _, x := range xs {
		result.inc(x, 1)
	}
	return result
}

func partOne(count *count) string {
	for range 25 {
		count.blink()
	}
	return fmt.Sprintf("%d", count.total())
}

func partTwo(count *count) string {
	for range 75 {
		count.blink()
	}
	return fmt.Sprintf("%d", count.total())
}

func Run(lines []string) (string, string, error) {
	return partOne(parse(lines)), partTwo(parse(lines)), nil
}
