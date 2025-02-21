const runner = require('../../utils/runner');

const parse = (input) => input.split('').filter((x) => x !== '\n');

const tiles = (row, i) => {
  if (i === 0) {
    return ['.', row[0], row[1]];
  }

  if (i === row.length - 1) {
    return [row[i - 1], row[i], '.'];
  }

  return [row[i - 1], row[i], row[i + 1]];
};

const isTrap = (x) => x === '^';

const run = (n) => (data) => {
  const rows = [data.slice()];
  while (rows.length < n) {
    const prev = rows.pop();

    const next = [];
    for (let i = 0; i < prev.length; i++) {
      const [l, c, r] = tiles(prev, i);
      const trap =
        (isTrap(l) && isTrap(c) && !isTrap(r)) ||
        (isTrap(c) && isTrap(r) && !isTrap(l)) ||
        (isTrap(l) && !isTrap(r) && !isTrap(c)) ||
        (isTrap(r) && !isTrap(l) && !isTrap(c));

      next.push(trap ? '^' : '.');
    }

    rows.push(prev, next);
  }

  return rows.reduce(
    (sum, row) => sum + row.filter((x) => !isTrap(x)).length,
    0
  );
};

const partOne = run(40);
const partTwo = run(400000);

runner(parse, partOne, partTwo);
