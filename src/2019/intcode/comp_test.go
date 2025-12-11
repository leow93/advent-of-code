package intcode

import (
	"testing"
)

func TestIntCode(t *testing.T) {
	t.Run("Day 2: add and multiply, position mode", func(t *testing.T) {
		p := []int{1, 9, 10, 3, 2, 3, 11, 0, 99, 30, 40, 50}
		err := RunProgram(p, nil, nil)
		if err != nil {
			t.Error("expected nil, got err", err)
		}
		if p[0] != 3500 {
			t.Errorf("expected 3500, got %d", p[0])
		}
	})

	t.Run("Day 5", func(t *testing.T) {
		p := []int{1101, 100, -1, 4, 0}
		err := RunProgram(p, nil, nil)
		if err != nil {
			t.Error("expected nil, got err", err)
		}
		if p[len(p)-1] != 99 {
			t.Errorf("expected 99, got %d", p[0])
		}
	})
}
