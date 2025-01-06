package main

import (
	"testing"

	"github.com/leow93/advent-of-code/2024/input"
)

func Test_PartTwo(t *testing.T) {
	lines, err := input.ReadLines("./test_input.txt")
	if err != nil {
		t.Error(err.Error())
	}

	rs, us, err := parse(lines)
	if err != nil {
		t.Error(err.Error())
	}

	t.Run("fixInvalid 1", func(t *testing.T) {
		pages := []int{75, 97, 47, 61, 53}
		fixed := fixInvalid(rs, []update{{pages}})

		if len(fixed) != 1 {
			t.Errorf("empty")
		}

		got := fixed[0].pages
		want := []int{97, 75, 47, 61, 53}
		if len(got) != len(want) {
			t.Errorf("expected len(%d), got len(%d)", len(want), len(got))
		}

		for i := range len(got) {
			if got[i] != want[i] {
				t.Errorf("expected %d at position %d, got %d", want[i], i, got[i])
			}
		}

		original := []int{75, 97, 47, 61, 53}
		for i, p := range pages {
			if p != original[i] {
				t.Errorf("expected %d, got %d", original[i], p)
			}
		}
	})

	t.Run("fixInvalid 2", func(t *testing.T) {
		pages := []int{61, 13, 29}
		fixed := fixInvalid(rs, []update{{pages}})

		if len(fixed) != 1 {
			t.Errorf("empty")
		}

		got := fixed[0].pages
		want := []int{61, 29, 13}
		if len(got) != len(want) {
			t.Errorf("expected len(%d), got len(%d)", len(want), len(got))
		}

		for i := range len(got) {
			if got[i] != want[i] {
				t.Errorf("expected %d at position %d, got %d", want[i], i, got[i])
			}
		}
	})

	t.Run("fixInvalid 3", func(t *testing.T) {
		pages := []int{97, 13, 75, 29, 47}
		fixed := fixInvalid(rs, []update{{pages}})

		if len(fixed) != 1 {
			t.Errorf("empty")
		}

		got := fixed[0].pages
		want := []int{97, 75, 47, 29, 13}
		if len(got) != len(want) {
			t.Errorf("expected len(%d), got len(%d)", len(want), len(got))
		}

		for i := range len(got) {
			if got[i] != want[i] {
				t.Errorf("expected %d at position %d, got %d", want[i], i, got[i])
			}
		}
	})

	t.Run("integration", func(t *testing.T) {
		result := partTwo(rs, us)
		if result != "123" {
			t.Errorf("expected %s, got %s", "123", result)
		}
	})
}
