const runner = require('../../utils/runner');

const parse = (input) => Number(input.split('\n')[0]);

const insertAt = (xs, i, x) => {
  return [...xs.slice(0, i), x, ...xs.slice(i)];
};

const partOne = (steps) => {
  let buffer = [0];
  let i = 1;
  let curr = 0;
  while (i < 2018) {
    const idx = (curr + steps) % buffer.length;
    buffer = insertAt(buffer, idx + 1, i);
    i++;
    curr = idx + 1;
  }

  return buffer[curr + 1];
};
const partTwo = (steps) => {
  let p = 0;
  let idx = 1;
  for (n = 1; n <= 50000000; n++) {
    p = (p + (steps % n) + 1) % n;
    if (p == 0) idx = n;
  }

  return idx;
};

runner(parse, partOne, partTwo);
