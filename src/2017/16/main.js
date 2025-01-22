const runner = require('../../utils/runner');

const parse = (input) => {
  return {
    program: Array.from({ length: 16 })
      .map((_, i) => String.fromCharCode(97 + i))
      .join(''),
    instructions: input.split('\n')[0].split(','),
  };
};

const spin = (str, n) => {
  const xs = str.split('');
  const start = xs.slice(0, xs.length - n);
  const end = xs.slice(-n);
  return [...end, ...start].join('');
};
const exchange = (str, a, b) => {
  const xs = str.split('');
  const result = xs.slice();
  const tmpA = result[a];
  const tmpB = result[b];
  result[b] = tmpA;
  result[a] = tmpB;
  return result.join('');
};
const partner = (str, a, b) => {
  const xs = str.split('');
  return exchange(
    str,
    xs.findIndex((x) => x === a),
    xs.findIndex((x) => x === b)
  );
};

const run = (program, instructions) => {
  return instructions.reduce((program, instruction) => {
    if (instruction.startsWith('s')) {
      const amount = Number(instruction.split('s')[1]);
      return spin(program, amount);
    }

    if (instruction.startsWith('x')) {
      const [a, b] = instruction.replace('x', '').split('/').map(Number);
      return exchange(program, a, b);
    }

    if (instruction.startsWith('p')) {
      const [a, b] = instruction.replace('p', '').split('/');
      return partner(program, a, b);
    }
    return program;
  }, program);
};

const runN = (n, p, instructions, history = []) => {
  for (
    let cycleLength = 0;
    cycleLength < n;
    cycleLength++, p = run(p, instructions)
  ) {
    if (history.includes(p)) return history[n % cycleLength];
    history.push(p);
  }
  return p;
};

const partOne = (data) => {
  return run(data.program, data.instructions);
};

const partTwo = (data) => {
  return runN(1_000_000_000, data.program, data.instructions, []);
};

runner(parse, partOne, partTwo);
