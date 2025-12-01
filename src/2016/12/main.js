const runner = require('../../utils/runner');

const parse = (input) => input.split('\n').filter((x) => x && x !== '\n');

const parseVal = (registers, x) => {
  if (x.toLowerCase() === x.toUpperCase()) {
    return Number(x);
  }
  return registers[x];
};

const run = (initialState) => (data) => {
  let state = { ...initialState };
  let pointer = 0;
  while (pointer < data.length) {
    const instruction = data[pointer];
    const [type, x, y] = instruction.split(' ');
    switch (type) {
      case 'cpy': {
        const xVal = parseVal(state, x);
        state[y] = xVal;
        pointer++;
        break;
      }
      case 'inc': {
        state[x] += 1;
        pointer++;
        break;
      }
      case 'dec': {
        state[x] -= 1;
        pointer++;
        break;
      }
      case 'jnz': {
        const xVal = parseVal(state, x);
        if (xVal === 0) {
          pointer++;
          break;
        }
        pointer += Number(y);
        break;
      }
    }
  }

  return state.a;
};

const partOne = run({ a: 0, b: 0, c: 0, d: 0 });
const partTwo = run({ a: 0, b: 0, c: 1, d: 0 });

runner(parse, partOne, partTwo);
