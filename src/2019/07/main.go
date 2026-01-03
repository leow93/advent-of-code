package main

import (
	"fmt"
	"strconv"
	"strings"

	"github.com/leow93/advent-of-code/2019/intcode"
	"github.com/leow93/advent-of-code/utils"
)

func parse(lines []string) []int64 {
	// just need first line
	line := lines[0]
	parts := strings.Split(line, ",")
	result := make([]int64, len(parts))

	for i, p := range parts {
		x, err := strconv.Atoi(p)
		if err != nil {
			panic(err)
		}
		result[i] = int64(x)
	}
	return result
}

func permute(nums []int) [][]int {
	res := make([][]int, 0)
	n := len(nums)
	var backtrack func(int)
	backtrack = func(first int) {
		if first == n {
			temp := make([]int, n)
			copy(temp, nums)
			res = append(res, temp)
		}
		for i := first; i < n; i++ {
			nums[first], nums[i] = nums[i], nums[first]
			backtrack(first + 1)
			nums[first], nums[i] = nums[i], nums[first]
		}
	}

	backtrack(0)
	return res
}

func partOne(p []int64) int64 {
	var max int64
	phases := permute([]int{0, 1, 2, 3, 4})
	for _, ph := range phases {
		comps := intcode.NewSeries(p, 5)
		if x := intcode.RunSeries(comps, 0, ph...); x > max {
			max = x
		}
	}
	return max
}

func partTwo(p []int64) int64 {
	var max int64
	phases := permute([]int{5, 6, 7, 8, 9})
	for _, ph := range phases {
		comps := intcode.NewSeries(p, 5)
		if output := intcode.RunSeriesLoop(comps, 0, ph...); output > max {
			max = output
		}
	}
	return max
}

func main() {
	lines := utils.ReadLines()
	p := parse(lines)
	fmt.Println("Part I", partOne(p))
	fmt.Println("Part II", partTwo(p))
}
