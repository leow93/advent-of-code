package intcode

import (
	"testing"
)

func TestIntCode(t *testing.T) {
	t.Run("Day 2: add and multiply, position mode", func(t *testing.T) {
		p := NewMemory([]int64{1, 9, 10, 3, 2, 3, 11, 0, 99, 30, 40, 50})
		err := RunProgram(p, nil, nil)
		if err != nil {
			t.Error("expected nil, got err", err)
		}
		if p.Get(0) != 3500 {
			t.Errorf("expected 3500, got %d", p.Get(0))
		}
	})

	t.Run("Day 5", func(t *testing.T) {
		p := NewMemory([]int64{1101, 100, -1, 4, 0})
		err := RunProgram(p, nil, nil)
		if err != nil {
			t.Error("expected nil, got err", err)
		}
		if p.Get(p.MaxKey()) != 99 {
			t.Errorf("expected 99, got %d", p.Get(p.MaxKey()))
		}
	})

	t.Run("Day 7: series", func(t *testing.T) {
		p := []int64{3, 15, 3, 16, 1002, 16, 10, 16, 1, 16, 15, 15, 4, 15, 99, 0, 0}
		comps := NewSeries(p, 5)
		phases := []int{4, 3, 2, 1, 0}
		result := RunSeries(comps, 0, phases...)
		if result != 43210 {
			t.Errorf("expected 43210, got %d", result)
		}
	})

	t.Run("Day 7: series loop", func(t *testing.T) {
		p := []int64{3, 26, 1001, 26, -4, 26, 3, 27, 1002, 27, 2, 27, 1, 27, 26, 27, 4, 27, 1001, 28, -1, 28, 1005, 28, 6, 99, 0, 0, 5}
		comps := NewSeries(p, 5)
		phases := []int{9, 8, 7, 6, 5}
		result := RunSeriesLoop(comps, 0, phases...)
		if result != 139629729 {
			t.Errorf("expected 139629729, got %d", result)
		}
	})
}
