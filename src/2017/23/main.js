const runner = require('../../utils/runner');

const parseArg = (x) => {
  const int = parseInt(x);
  if (isNaN(int)) {
    return {
      type: 'register',
      value: x,
    };
  }
  return {
    type: 'literal',
    value: int,
  };
};

const parse = (input) => {
  const lines = input.split('\n').filter((x) => x !== '');
  return lines.map((line) => {
    if (line.startsWith('set ')) {
      return {
        type: 'set',
        args: line.slice(4).split(' ').map(parseArg),
      };
    }

    if (line.startsWith('sub ')) {
      return {
        type: 'sub',
        args: line.slice(4).split(' ').map(parseArg),
      };
    }

    if (line.startsWith('mul ')) {
      return {
        type: 'mul',
        args: line.slice(4).split(' ').map(parseArg),
      };
    }

    if (line.startsWith('jnz ')) {
      return {
        type: 'jnz',
        args: line.slice(4).split(' ').map(parseArg),
      };
    }
  });
};

const loadArg = (registers, arg) => {
  if (arg.type === 'literal') {
    return arg.value;
  }

  return registers[arg.value];
};

const run = (registers, notifyMul, log) => (input) => {
  let i = 0;
  while (i < input.length) {
    log(i);
    const instr = input[i];
    if (instr.type === 'jnz') {
      const a = loadArg(registers, instr.args[0]);
      const b = loadArg(registers, instr.args[1]);
      if (a !== 0) {
        i += b;
      } else {
        i++;
      }
      continue;
    }

    const reg = instr.args[0].value;
    const value = loadArg(registers, instr.args[1]);

    if (instr.type === 'set') {
      registers[reg] = value;
    } else if (instr.type === 'mul') {
      registers[reg] = registers[reg] * value;
      notifyMul({ i, instr });
    } else if (instr.type === 'sub') {
      registers[reg] = registers[reg] - value;
    }

    i++;
  }
};

const initialState = () => ({
  a: 0,
  b: 0,
  c: 0,
  d: 0,
  e: 0,
  f: 0,
  g: 0,
  h: 0,
});

const partOne = (input) => {
  let count = 0;
  const notifyMul = () => count++;
  const runner = run(initialState(), notifyMul, () => {});
  runner(input);
  return count;
};

const partTwo = (input) => {
  const registers = {
    ...initialState(),
    a: 1,
  };
  let count = 0;
  const notifyMul = ({ i }) => {
    count++;
  };
  const runner = run(registers, notifyMul, console.log);
  runner(input);
  return registers;
};
runner(parse, partOne, partTwo);
