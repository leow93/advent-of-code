const runner = require('../../utils/runner');

const parse = (x) => x.split(',').map((range) => range.split('-').map(Number));

const repeatedNTimes = (n) => (x) => {
  const str = x.toString(10);
  const len = str.length;
  if (len % n !== 0) return false;

  const parts = [];
  for (let i = 0; i < n; i++) {
    const p = str.slice((i * len) / n, len / n + (i * len) / n);
    parts.push(p);
  }

  for (let i = 0; i < parts.length - 1; i++) {
    const curr = parts[i];
    const next = parts[i + 1];
    if (curr !== next) return false;
  }
  return true;
};

const repeatedAtLeastTwice = (n) => {
  const str = n.toString(10);
  const len = str.length;

  // can only have repeats up to length of string;
  for (let i = 2; i <= len; i++) {
    const f = repeatedNTimes(i);
    if (f(str)) return true;
  }
  return false;
};

const getInvalidIds = (invalidPredicate) => (start, end) => {
  const ids = [];
  for (let i = start; i <= end; i++) {
    if (invalidPredicate(i)) ids.push(i);
  }
  return ids;
};
const add = (a, b) => a + b;

const run = (invalidFn) => (ranges) => {
  const findInvalidIds = getInvalidIds(invalidFn);
  return ranges.reduce((sum, range) => {
    const invalidIds = findInvalidIds(range[0], range[1]);
    return invalidIds.reduce(add, sum);
  }, 0);
};

const partOne = run(repeatedNTimes(2));
const partTwo = run(repeatedAtLeastTwice);

runner(parse, partOne, partTwo);
