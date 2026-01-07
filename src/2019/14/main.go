package main

import (
	"fmt"
	"math"
	"strconv"
	"strings"

	"github.com/leow93/advent-of-code/utils"
)

type chemical struct {
	quantity int
	name     string
}

type reaction struct {
	in  []chemical
	out chemical
}

func parseChemical(c string) chemical {
	parts := strings.Split(c, " ")
	q, err := strconv.Atoi(parts[0])
	if err != nil {
		panic(err)
	}

	return chemical{
		quantity: q,
		name:     parts[1],
	}
}

func parseReaction(l string) reaction {
	parts := strings.Split(l, " => ")
	a, b := parts[0], parts[1]

	var in []chemical
	ins := strings.Split(a, ", ")
	for _, x := range ins {
		in = append(in, parseChemical(x))
	}

	return reaction{
		in:  in,
		out: parseChemical(b),
	}
}

func parse(lines []string) []reaction {
	result := make([]reaction, 0, len(lines))

	for _, l := range lines {
		result = append(result, parseReaction(l))
	}

	return result
}

func oreForFuel(data []reaction, fuel int) int {
	reactions := make(map[string]reaction)
	for _, r := range data {
		reactions[r.out.name] = r
	}

	needs := make(map[string]int)
	needs["FUEL"] = fuel
	leftovers := make(map[string]int)
	q := []string{"FUEL"}

	for len(q) > 0 {
		curr := q[0]
		q = q[1:]

		need := needs[curr]
		leftover := leftovers[curr]
		used := min(need, leftover)
		needs[curr] -= used
		leftovers[curr] -= used

		if needs[curr] == 0 {
			continue
		}
		r, ok := reactions[curr]
		if !ok {
			continue
		}
		runCount := int(math.Ceil(float64(needs[curr]) / float64(r.out.quantity)))

		produced := runCount * r.out.quantity
		leftovers[curr] += produced - needs[curr]

		for _, in := range r.in {
			needs[in.name] += runCount * in.quantity
			q = append(q, in.name)
		}

		needs[curr] = 0
	}

	return needs["ORE"]
}

func partOne(data []reaction) int {
	return oreForFuel(data, 1)
}

func partTwo(data []reaction) int {
	limit := 1_000_000_000_000
	lo := 1
	hi := 1
	// guess hi by doubling until it's too high
	for {
		ore := oreForFuel(data, hi)
		if ore <= limit {
			hi *= 2
			continue
		}
		break
	}

	for lo <= hi {
		mid := int(math.Floor((float64(lo) + float64(hi)) / 2))
		ore := oreForFuel(data, mid)

		if ore <= limit {
			lo = mid + 1
		} else {
			hi = mid - 1
		}

	}
	return hi
}

func main() {
	lines := utils.ReadLines()
	data := parse(lines)
	fmt.Println("Part I: ", partOne(data))
	fmt.Println("Part II: ", partTwo(data))
}
