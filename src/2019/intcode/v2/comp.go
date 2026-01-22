package v2

import (
	"fmt"
	"sync"
)

type Memory struct {
	mx   sync.RWMutex
	data map[int64]int64
	base int64
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
		mx:   sync.RWMutex{},
		data: data,
		base: 0,
	}
}

func (m *Memory) Get(i int64) int64 {
	m.mx.RLock()
	defer m.mx.RUnlock()
	return m.data[i]
}

func (m *Memory) Set(i int64, x int64) {
	m.mx.Lock()
	defer m.mx.Unlock()
	m.data[i] = x
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

type ProgramOpts struct {
	program *Memory
	input   <-chan int64
	output  chan<- int64
}

func NewProgramOpts(data []int64, input <-chan int64, output chan<- int64) *ProgramOpts {
	return &ProgramOpts{
		program: NewMemory(data),
		input:   input,
		output:  output,
	}
}

func RunProgram(opts *ProgramOpts) error {
	var i int64

	for {
		oc := parseOpcode(opts.program.Get(i))

		switch oc.opcode {
		case 99:
			return nil
		case 1:
			add(opts.program, i, oc.modes)
		case 2:
			mul(opts.program, i, oc.modes)
		case 3:
			i = saveFromInput(opts.program, i, opts.input, oc.modes)
			continue
		case 4:
			output(opts.program, i, oc.modes, opts.output)
		case 5:
			i = jit(opts.program, i, oc.modes)
			continue
		case 6:
			i = jif(opts.program, i, oc.modes)
			continue
		case 7:
			lt(opts.program, i, oc.modes)
		case 8:
			eq(opts.program, i, oc.modes)
		case 9:
			i = setBase(opts.program, i, oc.modes)
			continue
		default:
			return fmt.Errorf("failed at i=%d, opcode=%+v", i, oc)
		}

		i += int64(len(oc.modes) + 1)

	}
}
