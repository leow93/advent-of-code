const runner = require('../../utils/runner');
const parse = (input) =>
  input
    .split('\n')
    .filter((x) => x !== '' && x !== '\n')
    .map(Number);
const reduce = (f, initial) => (xs) => xs.reduce(f, initial);
const partOne = reduce((sum, x) => sum + x, 0);

const partTwo = (input) => {
  let frequency = 0;
  const seen = new Set([frequency]);
  let i = 0;
  while (true) {
    if (i >= input.length) i = 0;
    const change = input[i];
    frequency += change;
    if (seen.has(frequency)) {
      return frequency;
    }

    seen.add(frequency);

    i++;
  }
};

runner(parse, partOne, partTwo);
