package v2

import "fmt"

type mode int

const (
	position mode = iota
	immediate
	relative
)

func getValue(p *Memory, i int64, m mode) int64 {
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

func getWriteIndex(p *Memory, i int64, m mode) int64 {
	switch m {
	case relative:
		return p.Get(i) + p.GetBase()
	case position:
		return p.Get(i)
	default:
		panic(fmt.Sprintf("unexpected mode: %d", m))
	}
}

func add(p *Memory, i int64, m []mode) {
	aMode := m[0]
	a := getValue(p, i+1, aMode)
	bMode := m[1]
	b := getValue(p, i+2, bMode)
	idx := getWriteIndex(p, i+3, m[2])

	p.Set(idx, a+b)
}

func mul(p *Memory, i int64, m []mode) {
	aMode := m[0]
	a := getValue(p, i+1, aMode)
	bMode := m[1]
	b := getValue(p, i+2, bMode)
	idx := getWriteIndex(p, i+3, m[2])

	p.Set(idx, a*b)
}

func jit(p *Memory, i int64, m []mode) int64 {
	aMode := m[0]
	a := getValue(p, i+1, aMode)
	if a == 0 {
		return int64(i + 3)
	}
	bMode := m[1]
	b := getValue(p, i+2, bMode)
	return b
}

func jif(p *Memory, i int64, m []mode) int64 {
	aMode := m[0]
	a := getValue(p, i+1, aMode)
	if a != 0 {
		return int64(i + 3)
	}
	bMode := m[1]
	b := getValue(p, i+2, bMode)
	return b
}

func lt(p *Memory, i int64, m []mode) {
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

func eq(p *Memory, i int64, m []mode) {
	a := getValue(p, i+1, m[0])
	b := getValue(p, i+2, m[1])
	idx := getWriteIndex(p, i+3, m[2])

	if a == b {
		p.Set(idx, 1)
	} else {
		p.Set(idx, 0)
	}
}

func setBase(p *Memory, i int64, m []mode) int64 {
	a := getValue(p, i+1, m[0])
	base := p.GetBase()
	p.SetBase(base + a)
	return i + 2
}

func saveFromInput(p *Memory, i int64, input <-chan int64, modes []mode) int64 {
	x := <-input
	idx := getWriteIndex(p, i+1, modes[0])
	p.Set(idx, x)
	return i + 2
}

func output(p *Memory, i int64, m []mode, o chan<- int64) {
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
