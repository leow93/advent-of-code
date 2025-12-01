const runner = require('../../utils/runner');

const parse = (x) =>
  x.split('\n').map((line) => {
    if (line[0] === 'L') {
      return -Number(line.slice(1));
    }
    return Number(line.slice(1));
  });

const countEqualZero = (count, _, next) => count + (next % 100 === 0 ? 1 : 0);

const zeroesBetween = (start, end) => {
  const min = Math.min(start, end);
  const max = Math.max(start, end);

  const firstMultiple = Math.ceil(min / 100);
  const lastMultiple = Math.floor(max / 100);

  let count = Math.max(0, lastMultiple - firstMultiple + 1);

  // Exclude 'start' if it is a multiple of 100
  if (start % 100 === 0) {
    count--;
  }

  return Math.max(0, count);
};

const countAllZeroes = (count, prev, next) => count + zeroesBetween(prev, next);

const reduce = (counter) => (state, change) => {
  const next = state.current + change;
  return {
    current: next,
    count: counter(state.count, state.current, next),
  };
};

const run = (counter) => (lines) =>
  lines.reduce(reduce(counter), { count: 0, current: 50 }).count;

const partOne = run(countEqualZero);
const partTwo = run(countAllZeroes);

runner(parse, partOne, partTwo);
