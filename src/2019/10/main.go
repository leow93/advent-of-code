package main

import (
	"fmt"
	"math"
	"sort"

	"github.com/leow93/advent-of-code/utils"
)

func gcd(a, b int) int {
	// Work with absolute values
	if a < 0 {
		a = -a
	}
	if b < 0 {
		b = -b
	}

	for b != 0 {
		a, b = b, a%b
	}
	return a
}

func count(lines []string, x1, y1 int) int {
	// stores unique directions. if two asteroids lie on the same line, they produce the same key.
	seen := make(map[string]bool)

	for x2, line := range lines {
		for y2, ch := range line {
			if x2 == x1 && y2 == y1 {
				continue
			}

			if ch == '.' {
				continue
			}

			dx := x2 - x1
			dy := y2 - y1
			divisor := gcd(dx, dy)

			normalX := dx / divisor
			normalY := dy / divisor
			key := fmt.Sprintf("%d%d", normalX, normalY)
			seen[key] = true
		}
	}
	return len(seen)
}

func partOne(lines []string) (int, int, int) {
	max, x, y := 0, 0, 0

	for i, line := range lines {
		for j, c := range line {
			if c == '.' {
				continue
			}
			n := count(lines, i, j)
			if n > max {
				max = n
				x = i
				y = j
			}
		}
	}

	return max, x, y
}

type point struct {
	x, y int
}
type vector struct {
	dx, dy int
}

func getPoints(lines []string) []point {
	result := make([]point, 0)

	for i, line := range lines {
		for j, c := range line {
			if c == '.' {
				continue
			}
			result = append(result, point{i, j})
		}
	}
	return result
}

func getVectors(points []point, origin point) []vector {
	// 1. get all the normalised vectors that represent the lines to asteroids from the laser.
	vectors := make(map[vector]bool)

	for _, p := range points {
		if p.x == origin.x && p.y == origin.y {
			continue
		}

		dx := p.x - origin.x
		dy := p.y - origin.y
		divisor := gcd(dx, dy)

		normalX := dx / divisor
		normalY := dy / divisor
		vectors[vector{normalX, normalY}] = true
	}

	result := make([]vector, 0)
	for k := range vectors {
		result = append(result, k)
	}

	sort.Slice(result, func(i, j int) bool {
		// Calculate angles using the specific grid mapping:
		// math.Atan2(Opposite, Adjacent)
		// Opposite (Sine component) -> Col (Y)
		// Adjacent (Cosine component) -> -Row (-X)
		// This sets Top (-1, 0) as 0 radians
		angI := math.Atan2(float64(result[i].dy), float64(-result[i].dx))
		angJ := math.Atan2(float64(result[j].dy), float64(-result[j].dx))

		// Normalize negative angles (Left side of the clock)
		// This ensures angles go 0 -> PI -> 2PI
		if angI < 0 {
			angI += 2 * math.Pi
		}
		if angJ < 0 {
			angJ += 2 * math.Pi
		}

		return angI < angJ
	})

	return result
}

func vaporise(points []point, origin point, v vector, vaporised map[point]bool) (point, bool) {
	const depth = 1000

	for i := 1; i < depth; i++ {
		test := point{x: origin.x + i*v.dx, y: origin.y + i*v.dy}
		for _, p := range points {
			if vaporised[p] {
				continue
			}
			if p.x == test.x && p.y == test.y {
				return test, true
			}
		}
	}

	return point{}, false
}

func partTwo(lines []string, x, y int) int {
	origin := point{x, y}
	points := getPoints(lines)
	vectors := getVectors(points, origin)
	vaporised := make(map[point]bool)
	var last point
	i := 0
	count := 0
	for count < 200 {
		v := vectors[i]

		if p, ok := vaporise(points, origin, v, vaporised); ok {
			count++
			vaporised[p] = true
			last = p
		}
		i++
		if i == len(vectors) {
			i = 0
		}
	}

	return (100 * last.y) + last.x
}

func main() {
	lines := utils.ReadLines()
	c, x, y := partOne(lines)
	fmt.Printf("Part I: %d at (%d,%d)\n", c, x, y)
	fmt.Printf("Part II: %d\n", partTwo(lines, x, y))
}
