package main

import (
	"errors"
	"fmt"
	"strconv"
	"strings"

	"github.com/leow93/advent-of-code/utils"
)

func abs(x int) int {
	if x < 0 {
		return -x
	}
	return x
}

type moon struct {
	x, y, z, vx, vy, vz int
}

func (m *moon) String() string {
	return fmt.Sprintf("pos=<x=%d, y=%d, z=%d>, vel=<x=%d, y=%d, z=%d>\n", m.x, m.y, m.z, m.vx, m.vy, m.vz)
}

func (m *moon) xstring() string {
	return fmt.Sprintf("%d,%d", m.x, m.vx)
}

func (m *moon) ystring() string {
	return fmt.Sprintf("%d,%d", m.y, m.vy)
}

func (m *moon) zstring() string {
	return fmt.Sprintf("%d,%d", m.z, m.vz)
}

func (m *moon) applyV() {
	m.x += m.vx
	m.y += m.vy
	m.z += m.vz
}

func (m *moon) potential() int {
	return abs(m.x) + abs(m.y) + abs(m.z)
}

func (m *moon) kinetic() int {
	return abs(m.vx) + abs(m.vy) + abs(m.vz)
}

func (m *moon) total() int {
	return m.kinetic() * m.potential()
}

func newMoon(x, y, z int) *moon {
	return &moon{
		x:  x,
		y:  y,
		z:  z,
		vx: 0,
		vy: 0,
		vz: 0,
	}
}

func pairs[T any](xs []T) [][2]T {
	var result [][2]T

	for i := range xs {
		for j := i + 1; j < len(xs); j++ {
			result = append(result, [2]T{xs[i], xs[j]})
		}
	}

	return result
}

func parseLine(line string) (*moon, error) {
	s := strings.TrimPrefix(line, "<")
	s = strings.TrimSuffix(s, ">")
	xyz := strings.SplitN(s, ", ", 3)

	if len(xyz) != 3 {
		return nil, errors.New("invalid line")
	}

	x, err := strconv.Atoi(xyz[0][2:])
	if err != nil {
		return nil, err
	}
	y, err := strconv.Atoi(xyz[1][2:])
	if err != nil {
		return nil, err
	}
	z, err := strconv.Atoi(xyz[2][2:])
	if err != nil {
		return nil, err
	}
	return newMoon(x, y, z), nil
}

func parse(lines []string) []*moon {
	moons := make([]*moon, 0)

	for _, line := range lines {
		m, err := parseLine(line)
		if err != nil {
			panic(err)
		}
		moons = append(moons, m)
	}

	return moons
}

func deltaV(delta int) int {
	if delta > 0 {
		return 1
	}

	if delta < 0 {
		return -1
	}

	return 0
}

func simulateStep(moons []*moon) {
	// apply gravity
	ps := pairs(moons)
	for _, p := range ps {
		a, b := p[0], p[1]

		/*
		* For example, if Ganymede has an x position of 3, and Callisto has a x position of 5,
		* then Ganymede's x velocity changes by +1 (because 5 > 3)
		* and Callisto's x velocity changes by -1 (because 3 < 5).
		*
		* However, if the positions on a given axis are the same, the velocity on that axis does not change for that pair of moons.
		*
		 */
		dx, dy, dz := b.x-a.x, b.y-a.y, b.z-a.z
		dvx, dvy, dvz := deltaV(dx), deltaV(dy), deltaV(dz)
		a.vx += dvx
		b.vx -= dvx

		a.vy += dvy
		b.vy -= dvy

		a.vz += dvz
		b.vz -= dvz
	}
	// apply velocity
	for _, m := range moons {
		m.applyV()
	}
}

func partOne(lines []string) int {
	moons := parse(lines)
	steps := 1000

	for range steps {
		simulateStep(moons)
	}
	total := 0
	for _, m := range moons {
		total += m.total()
	}
	return total
}

func gcd(a, b int) int {
	for b != 0 {
		t := b
		b = a % b
		a = t
	}
	return a
}

func lcm(a, b int, integers ...int) int {
	result := a * b / gcd(a, b)

	for i := range integers {
		result = lcm(result, integers[i])
	}

	return result
}

func partTwo(lines []string) int {
	moons := parse(lines)
	xStates := make(map[string]bool)
	yStates := make(map[string]bool)
	zStates := make(map[string]bool)
	addStates := func() bool {
		x, y, z := "", "", ""
		for _, m := range moons {
			x += m.xstring()
			y += m.ystring()
			z += m.zstring()
		}

		_, xSeen := xStates[x]
		_, ySeen := yStates[y]
		_, zSeen := zStates[z]

		if xSeen && ySeen && zSeen {
			return true
		}

		xStates[x] = true
		yStates[y] = true
		zStates[z] = true
		return false
	}
	addStates()

	for {
		simulateStep(moons)
		if addStates() {
			break
		}
	}

	return lcm(len(xStates), lcm(len(yStates), len(zStates)))
}

func main() {
	lines := utils.ReadLines()
	fmt.Println("Part I: ", partOne(lines))
	fmt.Println("Part II: ", partTwo(lines))
}
