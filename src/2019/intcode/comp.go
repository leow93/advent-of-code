package intcode

import (
	"fmt"
	"sync"
)

type mode int

const (
	position mode = iota
	immediate
	relative
)

func getValue(p *memory, i int64, m mode) int64 {
	switch m {
	case immediate:
		return p.Get(i)
	case relative:
		return p.Get(p.Get(i) + p.GetBase())
	case position:
		return p.Get(p.Get(i))
	default:
		panic(fmt.Sprintf("unexpected mode: %d", m))
	}
}

func getWriteIndex(p *memory, i int64, m mode) int64 {
	switch m {
	case relative:
		return p.Get(i) + p.GetBase()
	case position:
		return p.Get(i)
	default:
		panic(fmt.Sprintf("unexpected mode: %d", m))
	}
}

func add(p *memory, i int64, m []mode) {
	aMode := m[0]
	a := getValue(p, i+1, aMode)
	bMode := m[1]
	b := getValue(p, i+2, bMode)
	idx := getWriteIndex(p, i+3, m[2])

	p.Set(idx, a+b)
}

func mul(p *memory, i int64, m []mode) {
	aMode := m[0]
	a := getValue(p, i+1, aMode)
	bMode := m[1]
	b := getValue(p, i+2, bMode)
	idx := getWriteIndex(p, i+3, m[2])

	p.Set(idx, a*b)
}

func jit(p *memory, i int64, m []mode) int64 {
	aMode := m[0]
	a := getValue(p, i+1, aMode)
	if a == 0 {
		return int64(i + 3)
	}
	bMode := m[1]
	b := getValue(p, i+2, bMode)
	return b
}

func jif(p *memory, i int64, m []mode) int64 {
	aMode := m[0]
	a := getValue(p, i+1, aMode)
	if a != 0 {
		return int64(i + 3)
	}
	bMode := m[1]
	b := getValue(p, i+2, bMode)
	return b
}

func lt(p *memory, i int64, m []mode) {
	aMode := m[0]
	a := getValue(p, i+1, aMode)
	bMode := m[1]
	b := getValue(p, i+2, bMode)
	idx := getWriteIndex(p, i+3, m[2])
	if a < b {
		p.Set(idx, 1)
	} else {
		p.Set(idx, 0)
	}
}

func eq(p *memory, i int64, m []mode) {
	a := getValue(p, i+1, m[0])
	b := getValue(p, i+2, m[1])
	idx := getWriteIndex(p, i+3, m[2])

	if a == b {
		p.Set(idx, 1)
	} else {
		p.Set(idx, 0)
	}
}

func setBase(p *memory, i int64, m []mode) int64 {
	a := getValue(p, i+1, m[0])
	base := p.GetBase()
	p.SetBase(base + a)
	return i + 2
}

func saveFromInput(p *memory, i int64, input <-chan int64, modes []mode) int64 {
	x := <-input
	idx := getWriteIndex(p, i+1, modes[0])
	p.Set(idx, x)
	return i + 2
}

func output(p *memory, i int64, m []mode, o chan<- int64) {
	o <- getValue(p, i+1, m[0])
}

func getDigit(num, position int64) int64 {
	if num < 0 {
		num = -num
	}

	for range position {
		num /= 10
	}

	return num % 10
}

type opcode struct {
	opcode int64
	modes  []mode
}

func parseOpcode(code int64) opcode {
	if code == 99 {
		return opcode{99, nil}
	}

	if code == 3 {
		return opcode{3, []mode{position}}
	}
	digitsRTL := []int64{
		getDigit(code, 0),
		getDigit(code, 1),
		getDigit(code, 2),
		getDigit(code, 3),
		getDigit(code, 4),
	}

	oc := digitsRTL[0] + (10 * digitsRTL[1])
	if oc == 4 {
		return opcode{4, []mode{mode(digitsRTL[2])}}
	}

	modes := make([]mode, 3)
	for i := 2; i < len(digitsRTL); i++ {
		modes[i-2] = mode(digitsRTL[i])
	}

	return opcode{oc, modes}
}

type Computer struct {
	memory       *memory
	input        chan int64
	output       chan int64
	closed       bool
	relativeBase int64
	mx           sync.RWMutex
}

func New(program []int64, input chan int64, output chan int64) *Computer {
	p := make([]int64, len(program))
	copy(p, program)
	relativeBase := int64(0)
	return &Computer{NewMemory(p), input, output, false, relativeBase, sync.RWMutex{}}
}

func (c *Computer) stop() {
	close(c.input)
	close(c.output)
	c.closed = true
}

type inputMsg struct {
	msgType string
	data    int64
}

func instructionMsg(data int64) inputMsg {
	return inputMsg{
		msgType: "instruction",
		data:    data,
	}
}

func shutdownMsg() inputMsg {
	return inputMsg{
		msgType: "shutdown",
	}
}

func (c *Computer) executeProgram() error {
	defer func() {
		c.sendInput(shutdownMsg())
	}()
	return RunProgram(c.memory, c.input, c.output)
}

func (c *Computer) sendInput(msg inputMsg) {
	c.mx.Lock()
	defer c.mx.Unlock()
	if c.closed {
		return
	}

	switch msg.msgType {
	case "instruction":
		c.input <- msg.data
		return
	case "shutdown":
		c.stop()
		return
	}
}

func NewSeries(program []int64, n int) []*Computer {
	if n < 1 {
		return nil
	}
	comps := make([]*Computer, n)
	for i := range n {
		comps[i] = New(program, make(chan int64), make(chan int64))
	}
	return comps
}

type loopMsg struct {
	dest int
	data int64
}

func runSeries(comps []*Computer, nextComp func(idx int) (int, *Computer), input int64, phases ...int) int64 {
	q := make(chan loopMsg)
	qdone := make(chan struct{})
	var result int64
	phasesWg := sync.WaitGroup{}
	phasesWg.Add(len(comps))

	go func() {
		for msg := range q {
			c := comps[msg.dest]
			c.sendInput(instructionMsg(msg.data))
		}
		qdone <- struct{}{}
	}()

	// set phases on the machines
	for idx, comp := range comps {
		go func(i int, c *Computer) {
			defer phasesWg.Done()
			if idx < len(phases) {
				comp.sendInput(instructionMsg(int64(phases[idx])))
			}
			// send inital input to first computer
			if idx == 0 {
				comp.sendInput(instructionMsg(input))
			}
		}(idx, comp)
	}

	executionWg := sync.WaitGroup{}
	executionWg.Add(len(comps))
	// run
	for idx, comp := range comps {
		go func(i int, c *Computer) {
			defer executionWg.Done()
			err := c.executeProgram()
			if err != nil {
				panic(err)
			}
		}(idx, comp)
	}

	// wait for phases to be set
	phasesWg.Wait()

	outWg := sync.WaitGroup{}
	outWg.Add(len(comps))
	// pipe output int64o next computer
	for idx, comp := range comps {
		go func(i int, c *Computer) {
			defer outWg.Done()
			for x := range comp.output {
				if i == len(comps)-1 {
					result = x
				}

				nextIdx, nextComp := nextComp(i)
				if nextComp != nil {
					q <- loopMsg{
						dest: nextIdx,
						data: x,
					}
				}

			}
		}(idx, comp)
	}
	executionWg.Wait()
	outWg.Wait()
	close(q)
	<-qdone

	return result
}

func RunSeries(comps []*Computer, input int64, phases ...int) int64 {
	nextComp := func(idx int) (int, *Computer) {
		if idx >= len(comps)-1 {
			return -1, nil
		}
		return idx + 1, comps[idx+1]
	}
	return runSeries(comps, nextComp, input, phases...)
}

func RunSeriesLoop(comps []*Computer, input int64, phases ...int) int64 {
	nextComp := func(idx int) (int, *Computer) {
		if idx >= len(comps)-1 {
			return 0, comps[0]
		}
		return idx + 1, comps[idx+1]
	}
	return runSeries(comps, nextComp, input, phases...)
}

func RunProgram(program *memory, input <-chan int64, o chan<- int64) error {
	var i int64

	for {
		oc := parseOpcode(program.Get(i))

		switch oc.opcode {
		case 99:
			return nil
		case 1:
			add(program, i, oc.modes)
		case 2:
			mul(program, i, oc.modes)
		case 3:
			i = saveFromInput(program, i, input, oc.modes)
			continue
		case 4:
			output(program, i, oc.modes, o)
		case 5:
			i = jit(program, i, oc.modes)
			continue
		case 6:
			i = jif(program, i, oc.modes)
			continue
		case 7:
			lt(program, i, oc.modes)
		case 8:
			eq(program, i, oc.modes)
		case 9:
			i = setBase(program, i, oc.modes)
			continue
		default:
			return fmt.Errorf("failed at i=%d, opcode=%+v", i, oc)
		}

		i += int64(len(oc.modes) + 1)

	}
}
