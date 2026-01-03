package intcode

import "sync"

type memory struct {
	mx     sync.RWMutex
	data   map[int64]int64
	maxKey int64
}

func NewMemory(program []int64) *memory {
	data := make(map[int64]int64)
	maxKey := int64(0)
	for i, x := range program {
		data[int64(i)] = x
		if int64(i) > maxKey {
			maxKey = int64(i)
		}
	}
	return &memory{
		mx:     sync.RWMutex{},
		data:   data,
		maxKey: maxKey,
	}
}

func (m *memory) Get(i int64) int64 {
	m.mx.RLock()
	defer m.mx.RUnlock()
	x, _ := m.data[i]
	return x
}

func (m *memory) Set(i int64, x int64) {
	m.mx.Lock()
	defer m.mx.Unlock()
	m.data[i] = x
	if i > m.maxKey {
		m.maxKey = i
	}
}

func (m *memory) MaxKey() int64 {
	m.mx.RLock()
	defer m.mx.RUnlock()
	return m.maxKey
}
