const runner = require('../../utils/runner');

const parse = (input) =>
  input
    .split('\n')
    .map((x) => x.split(' '))
    .filter((x) => x.length > 1);

const partOne = (data) => {
  const registers = {};
  const sounds = [];
  let i = 0;

  while (true) {
    const [type, reg, value] = data[i];
    if (registers[reg] === undefined) registers[reg] = 0;
    let x =
      value !== undefined
        ? value.toLowerCase() !== value.toUpperCase()
          ? registers[value] ?? 0
          : parseInt(value)
        : undefined;

    if (type === 'jgz') {
      if (registers[reg] <= 0) {
        i++;
        continue;
      }
      i += x;
      continue;
    }

    switch (type) {
      case 'set':
        registers[reg] = x;
        break;
      case 'snd':
        sounds.push(registers[reg]);
        break;
      case 'add':
        registers[reg] += x;
        break;
      case 'mul':
        registers[reg] *= x;
        break;
      case 'mod':
        registers[reg] %= x;
        break;
      case 'rcv':
        if (x === 0) break;
        return sounds.pop();
    }
    i += 1;
  }
};

const partTwo = (data) => {
  const programZero = {
    id: 0,
    registers: { p: 0 },
    queue: [],
    pointer: 0,
    sendCount: 0,
    waiting: false,
  };
  const programOne = {
    id: 1,
    registers: { p: 1 },
    queue: [],
    pointer: 0,
    sendCount: 0,
    waiting: false,
  };

  const getValue = (state, arg) => {
    return isNaN(arg) ? state.registers[arg] ?? 0 : Number(arg);
  };

  function run(state, otherState) {
    if (state.pointer < 0 || state.pointer >= data.length) {
      state.waiting = true;
      return;
    }

    const [type, reg, value] = data[state.pointer];
    if (state.registers[reg] === undefined) state.registers[reg] = 0;
    const x = getValue(state, value);
    switch (type) {
      case 'snd':
        otherState.queue.push(getValue(state, reg));
        state.sendCount++;
        break;
      case 'set':
        state.registers[reg] = x;
        break;
      case 'add':
        state.registers[reg] += x;
        break;
      case 'mul':
        state.registers[reg] *= x;
        break;
      case 'mod':
        state.registers[reg] %= x;
        break;
      case 'rcv':
        if (state.queue.length > 0) {
          state.registers[reg] = state.queue.shift();
          state.waiting = false;
        } else {
          state.waiting = true;
          return;
        }
        break;
      case 'jgz':
        if (getValue(state, reg) > 0) {
          state.pointer += x - 1;
        }
        break;
    }

    state.pointer++;
  }

  let bothWaiting = false;
  let bothTerminated = false;

  while (!(bothWaiting || bothTerminated)) {
    run(programZero, programOne);
    run(programOne, programZero);

    bothWaiting = programZero.waiting && programOne.waiting;
    bothTerminated =
      programZero.pointer >= data.length && programOne.pointer >= data.length;
  }

  return programOne.sendCount;
};

runner(parse, partOne, partTwo);
