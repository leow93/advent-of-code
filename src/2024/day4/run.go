package day4

import (
	"fmt"
)

type grid [][]rune

func parseGrid(input []string) grid {
	var result [][]rune
	for _, line := range input {
		row := []rune(line)
		result = append(result, row)
	}
	return result
}

func east(coord [2]int) [][2]int {
	return [][2]int{
		{coord[0], coord[1]},
		{coord[0], coord[1] + 1},
		{coord[0], coord[1] + 2},
		{coord[0], coord[1] + 3},
	}
}

func north(coord [2]int) [][2]int {
	return [][2]int{
		{coord[0], coord[1]},
		{coord[0] - 1, coord[1]},
		{coord[0] - 2, coord[1]},
		{coord[0] - 3, coord[1]},
	}
}

func south(coord [2]int) [][2]int {
	return [][2]int{
		{coord[0], coord[1]},
		{coord[0] + 1, coord[1]},
		{coord[0] + 2, coord[1]},
		{coord[0] + 3, coord[1]},
	}
}

func west(coord [2]int) [][2]int {
	return [][2]int{
		{coord[0], coord[1]},
		{coord[0], coord[1] - 1},
		{coord[0], coord[1] - 2},
		{coord[0], coord[1] - 3},
	}
}

func northEast(coord [2]int) [][2]int {
	return [][2]int{
		{coord[0], coord[1]},
		{coord[0] - 1, coord[1] + 1},
		{coord[0] - 2, coord[1] + 2},
		{coord[0] - 3, coord[1] + 3},
	}
}

func northWest(coord [2]int) [][2]int {
	return [][2]int{
		{coord[0], coord[1]},
		{coord[0] - 1, coord[1] - 1},
		{coord[0] - 2, coord[1] - 2},
		{coord[0] - 3, coord[1] - 3},
	}
}

func southEast(coord [2]int) [][2]int {
	return [][2]int{
		{coord[0], coord[1]},
		{coord[0] + 1, coord[1] + 1},
		{coord[0] + 2, coord[1] + 2},
		{coord[0] + 3, coord[1] + 3},
	}
}

func southWest(coord [2]int) [][2]int {
	return [][2]int{
		{coord[0], coord[1]},
		{coord[0] + 1, coord[1] - 1},
		{coord[0] + 2, coord[1] - 2},
		{coord[0] + 3, coord[1] - 3},
	}
}

func (g *grid) paths(i, j int) [][][2]int {
	grid := *g
	paths := [][][2]int{}

	coord := [2]int{i, j}

	if i >= 3 {
		paths = append(paths, north(coord))

		if j < len(grid[0])-3 {
			paths = append(paths, northEast(coord))
		}
		if j >= 3 {
			paths = append(paths, northWest(coord))
		}
	}

	if i < len(grid)-3 {
		paths = append(paths, south(coord))

		if j < len(grid[0])-3 {
			paths = append(paths, southEast(coord))
		}

		if j >= 3 {
			paths = append(paths, southWest(coord))
		}

	}

	if j < len(grid[0])-3 {
		paths = append(paths, east(coord))
	}

	if j >= 3 {
		paths = append(paths, west(coord))
	}

	return paths
}

func (g *grid) searchXmas(i, j int) int {
	grid := *g
	paths := g.paths(i, j)
	count := 0
	word := "XMAS"

	for _, p := range paths {
		w := ""
		for i := 0; i < len(p); i++ {
			x := p[i][0]
			y := p[i][1]
			want := rune(word[i])

			if grid[x][y] != want {
				break
			}
			w += string(grid[x][y])
		}
		if w == word {
			count += 1
		}
	}
	return count
}

func partOne(g grid) string {
	count := 0
	for i, row := range g {
		for j := range row {
			count += g.searchXmas(i, j)
		}
	}

	return fmt.Sprintf("%d", count)
}

func partTwo(g grid) string {
	count := 0
	for i := 1; i < len(g)-1; i++ {
		for j := 1; j < len(g[0])-1; j++ {
			if g[i][j] != 'A' {
				continue
			}

			tl := g[i-1][j-1]
			tr := g[i-1][j+1]
			bl := g[i+1][j-1]
			br := g[i+1][j+1]

			xmas := ((tl == 'M' && br == 'S') || (tl == 'S' && br == 'M')) && ((tr == 'M' && bl == 'S') || (tr == 'S' && bl == 'M'))
			if xmas {
				count += 1
			}
		}
	}

	return fmt.Sprintf("%d", count)
}

func Run(input []string) (string, string, error) {
	grid := parseGrid(input)
	one := partOne(grid)
	two := partTwo(grid)

	return one, two, nil
}
