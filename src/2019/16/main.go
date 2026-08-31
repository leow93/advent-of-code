package main

import (
	"fmt"

	"github.com/leow93/advent-of-code/utils"
)

type data struct {
	input     []int
	overrides map[int]int
}

func newData(input []int) *data {
	return &data{
		input:     input,
		overrides: make(map[int]int),
	}
}

func (d *data) Clone() *data {
	input := make([]int, len(d.input))
	copy(input, d.input)

	result := newData(input)

	for i := range d.input {
		result.Set(i, d.Get(i))
	}
	return result
}

func (d *data) Get(idx int) int {
	if x, ok := d.overrides[idx]; ok {
		return x
	}

	return d.input[idx]
}

func (d *data) Set(idx int, value int) {
	d.overrides[idx] = value
}

func parse(lines []string) []int {
	line := lines[0]
	result := make([]int, len(line))

	for i, c := range line {
		result[i] = int(c - '0')
	}
	return result
}

func resolvePatternElm(repeat int, i int) int {
	base := []int{0, 1, 0, -1}
	j := ((1 + i) / repeat) % 4
	return base[j]
}

func abs(x int) int {
	if x < 0 {
		return -x
	}
	return x
}

func ones(x int) int {
	if abs(x) < 10 {
		return abs(x)
	}

	return abs(x % 10)
}

func fft(data *data, phases int) {
	debug := true
	curr := data
	for p := range phases {
		if debug {
			fmt.Printf("After %d phases\n", p)
		}
		str := ""
		for i := range data.input {
			total := 0

			for j := range data.input {
				fmt.Println(i, j)
				rpe := resolvePatternElm(i+1, j)
				y := data.Get(j) * rpe
				str += fmt.Sprintf("%d*%d ", data.Get(j), rpe)
				if j < len(data.input)-1 {
					str += " + "
				}
				total += y
			}
			str += fmt.Sprintf(" = %d\n", ones(total))
			curr.Set(i, ones(total))
		}

		if debug {
			fmt.Print(str)
		}

		*data = *curr
	}
}

func join(xs []int) string {
	result := ""
	for i := range xs {
		result += fmt.Sprintf("%d", xs[i])
	}
	return result
}

func partOne(input []int) string {
	data := newData(input)
	fft(data, 100)
	result := ""
	for i := range data.input[0:8] {
		result += fmt.Sprintf("%d", data.Get(i))
	}

	return result
}

func main() {
	lines := utils.ReadLines()
	input := parse(lines)
	p1 := partOne(input)
	if p1 != "10189359" {
		fmt.Printf("got %s, want %s\n", p1, "10189359")
		return
	}
	fmt.Print("Part I:", partOne(input))
}
