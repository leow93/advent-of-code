package intcode

import (
	"fmt"
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

func saveFromInput(p []int, i int, input <-chan int) {
	x := <-input
	idx := p[i+1]
	p[idx] = x
}

func output(p []int, i int, m []mode) {
	a := getValue(p, i+1, m[0])
	fmt.Println(a)
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

func RunProgram(program []int, input <-chan int) error {
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
			output(program, i, oc.modes)

		default:
			return fmt.Errorf("failed at i=%d, opcode=%+v", i, oc)
		}

		i += len(oc.modes) + 1
	}
}
