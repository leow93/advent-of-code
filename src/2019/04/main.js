const runner = require('../../utils/runner');

const parse = (x) => x.split('-').map(Number);
const isValidPartOne = (n) => {
  const x = n.toString();
  if (x.length !== 6) return false;
  let hasAdjacentDigits = false;

  for (let i = 0; i < x.length - 1; i++) {
    const a = x[i];
    const b = x[i + 1];
    if (a === b) hasAdjacentDigits = true;
    if (Number(a) > Number(b)) return false;
  }

  return hasAdjacentDigits;
};

const isValidPartTwo = (n) => {
  const x = n.toString();
  if (x.length !== 6) return false;
  const adjacentDigits = {};

  for (let i = 0; i < x.length - 1; i++) {
    const a = x[i];
    const b = x[i + 1];
    if (a === b) {
      adjacentDigits[a] = adjacentDigits[a] ?? [];
      adjacentDigits[a].push(i, i + 1);
    }
    if (Number(a) > Number(b)) return false;
  }

  return Object.values(adjacentDigits).some((value) => value.length === 2);
};

const run =
  (valid) =>
  ([lo, hi]) => {
    let count = 0;
    for (let i = lo; i <= hi; i++) {
      if (valid(i)) count++;
    }
    return count;
  };

const partOne = run(isValidPartOne);
const partTwo = run(isValidPartTwo);

runner(parse, partOne, partTwo);
