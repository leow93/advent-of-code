package intcode

import (
	"sync"
)

type Memory struct {
	mx     sync.RWMutex
	data   map[int64]int64
	maxKey int64
	base   int64
}

func NewMemory(program []int64) *Memory {
	data := make(map[int64]int64)
	maxKey := int64(0)
	for i, x := range program {
		data[int64(i)] = x
		if int64(i) > maxKey {
			maxKey = int64(i)
		}
	}
	return &Memory{
		mx:     sync.RWMutex{},
		data:   data,
		maxKey: maxKey,
		base:   0,
	}
}

func (m *Memory) Get(i int64) int64 {
	m.mx.RLock()
	defer m.mx.RUnlock()
	x, _ := m.data[i]
	return x
}

func (m *Memory) Set(i int64, x int64) {
	m.mx.Lock()
	defer m.mx.Unlock()
	m.data[i] = x
	if i > m.maxKey {
		m.maxKey = i
	}
}

func (m *Memory) SetBase(x int64) {
	m.mx.Lock()
	defer m.mx.Unlock()
	m.base = x
}

func (m *Memory) GetBase() int64 {
	m.mx.RLock()
	defer m.mx.RUnlock()
	return m.base
}

func (m *Memory) MaxKey() int64 {
	m.mx.RLock()
	defer m.mx.RUnlock()
	return m.maxKey
}
