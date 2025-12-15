package intcode

import (
	"fmt"
	"sync"
)

type mode int

const (
	position mode = iota
	immediate
)

func getValue(p []int, i int, m mode) int {
	if m == immediate {
		return p[i]
	}

	return p[p[i]]
}

func add(p []int, i int, m []mode) {
	aMode := m[0]
	a := getValue(p, i+1, aMode)
	bMode := m[1]
	b := getValue(p, i+2, bMode)
	idx := p[i+3]

	p[idx] = a + b
}

func mul(p []int, i int, m []mode) {
	aMode := m[0]
	a := getValue(p, i+1, aMode)
	bMode := m[1]
	b := getValue(p, i+2, bMode)
	idx := p[i+3]

	p[idx] = a * b
}

func jit(p []int, i int, m []mode) int {
	aMode := m[0]
	a := getValue(p, i+1, aMode)
	if a == 0 {
		return i + 3
	}
	bMode := m[1]
	b := getValue(p, i+2, bMode)
	return b
}

func jif(p []int, i int, m []mode) int {
	aMode := m[0]
	a := getValue(p, i+1, aMode)
	if a != 0 {
		return i + 3
	}
	bMode := m[1]
	b := getValue(p, i+2, bMode)
	return b
}

func lt(p []int, i int, m []mode) {
	aMode := m[0]
	a := getValue(p, i+1, aMode)
	bMode := m[1]
	b := getValue(p, i+2, bMode)
	idx := p[i+3]
	if a < b {
		p[idx] = 1
	} else {
		p[idx] = 0
	}
}

func eq(p []int, i int, m []mode) {
	aMode := m[0]
	a := getValue(p, i+1, aMode)
	bMode := m[1]
	b := getValue(p, i+2, bMode)
	idx := p[i+3]
	if a == b {
		p[idx] = 1
	} else {
		p[idx] = 0
	}
}

func saveFromInput(p []int, i int, input <-chan int) {
	x := <-input
	idx := p[i+1]
	p[idx] = x
}

func output(p []int, i int, m []mode, o chan<- int) {
	o <- getValue(p, i+1, m[0])
}

func getDigit(num, position int) int {
	if num < 0 {
		num = -num
	}

	for range position {
		num /= 10
	}

	return num % 10
}

type opcode struct {
	opcode int
	modes  []mode
}

func parseOpcode(code int) opcode {
	if code == 99 {
		return opcode{99, nil}
	}

	if code == 3 {
		return opcode{3, []mode{position}}
	}
	digitsRTL := []int{
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
	program []int
	input   chan int
	output  chan int
	closed  bool
	mx      sync.RWMutex
}

func New(program []int, input chan int, output chan int) *Computer {
	p := make([]int, len(program))
	copy(p, program)
	return &Computer{p, input, output, false, sync.RWMutex{}}
}

func (c *Computer) stop() {
	close(c.input)
	close(c.output)
	c.closed = true
}

type inputMsg struct {
	msgType string
	data    int
}

func instructionMsg(data int) inputMsg {
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
	return RunProgram(c.program, c.input, c.output)
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

func NewSeries(program []int, n int) []*Computer {
	if n < 1 {
		return nil
	}
	comps := make([]*Computer, n)
	for i := range n {
		comps[i] = New(program, make(chan int), make(chan int))
	}
	return comps
}

type loopMsg struct {
	dest int
	data int
}

func runSeries(comps []*Computer, nextComp func(idx int) (int, *Computer), input int, phases ...int) int {
	q := make(chan loopMsg)
	qdone := make(chan struct{})
	var result int
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
				comp.sendInput(instructionMsg(phases[idx]))
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
	// pipe output into next computer
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

func RunSeries(comps []*Computer, input int, phases ...int) int {
	nextComp := func(idx int) (int, *Computer) {
		if idx >= len(comps)-1 {
			return -1, nil
		}
		return idx + 1, comps[idx+1]
	}
	return runSeries(comps, nextComp, input, phases...)
}

func RunSeriesLoop(comps []*Computer, input int, phases ...int) int {
	nextComp := func(idx int) (int, *Computer) {
		if idx >= len(comps)-1 {
			return 0, comps[0]
		}
		return idx + 1, comps[idx+1]
	}
	return runSeries(comps, nextComp, input, phases...)
}

func RunProgram(program []int, input <-chan int, o chan<- int) error {
	i := 0

	for {
		oc := parseOpcode(program[i])

		switch oc.opcode {
		case 99:
			return nil
		case 1:
			add(program, i, oc.modes)
		case 2:
			mul(program, i, oc.modes)
		case 3:
			saveFromInput(program, i, input)
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

		default:
			return fmt.Errorf("failed at i=%d, opcode=%+v", i, oc)
		}

		i += len(oc.modes) + 1
	}
}
