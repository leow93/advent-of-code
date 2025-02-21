const runner = require('../../utils/runner');

const parse = Number;

const eq = ([a, b], [c, d]) => a === c && b === d;

const neighbours = [
  [-1, 0],
  [1, 0],
  [0, 1],
  [0, -1],
];

const wallCache = new Map();

const isWall = (y, x, n) => {
  const k = `${y},${x},${n}`;
  if (wallCache.has(k)) return wallCache.get(k);

  const a = x * x + 3 * x + 2 * x * y + y + y * y;
  const b = a + n;
  const binary = b.toString(2);
  const ones = binary.split('').filter((x) => x === '1').length;
  const result = ones % 2 !== 0;
  wallCache.set(k, result);
  return result;
};

const partOne = (n) => {
  const queue = [{ steps: 0, coord: [1, 1] }];
  const end = [39, 31];
  const visited = new Set();
  while (queue.length) {
    const curr = queue.shift();
    if (eq(curr.coord, end)) return curr.steps;
    const [y, x] = curr.coord;
    const k = `${y},${x}`;
    if (visited.has(k)) continue;
    visited.add(k);

    for (const [dy, dx] of neighbours) {
      const nextY = y + dy;
      const nextX = x + dx;
      if (nextY < 0 || nextX < 0) {
        continue;
      }
      // is it a wall?
      if (isWall(nextY, nextX, n)) continue;

      queue.push({ steps: curr.steps + 1, coord: [nextY, nextX] });
    }
  }
};

class Grid {
  constructor() {
    this.data = [];
  }
  set(y, x, value) {
    while (this.data.length <= y) {
      this.data.push(Array.from({ length: x }).fill('.'));
    }

    const data = this.data.map((row) => {
      const r = row.slice();
      while (r.length <= x) {
        r.push('.');
      }
      return r;
    });

    data[y][x] = value;
    this.data = data;
  }
  print() {
    for (const row of this.data) {
      console.log(row.join(''));
    }
  }
}

const partTwo = (n) => {
  const grid = new Grid();
  const queue = [{ steps: 0, coord: [1, 1] }];
  const visited = new Set();
  let i = 0;
  while (queue.length) {
    i++;
    grid.print();
    console.log();
    const now = Date.now();
    while (Date.now() < now + 200) {
      Math.random();
    }
    const curr = queue.pop();
    if (curr.steps > 10) {
      continue;
    }
    const [y, x] = curr.coord;
    const k = `${y},${x}`;
    if (visited.has(k)) continue;
    visited.add(k);
    grid.set(y, x, 'O');

    for (const [dy, dx] of neighbours) {
      const nextY = y + dy;
      const nextX = x + dx;
      if (nextY < 0 || nextX < 0) {
        continue;
      }
      // is it a wall?
      if (isWall(nextY, nextX, n)) {
        grid.set(nextY, nextX, '#');
        continue;
      }

      queue.push({ steps: curr.steps + 1, coord: [nextY, nextX] });
    }
  }

  grid.print();

  // 118 too low
  // 120 ?
  return visited.size;
};

runner(parse, partOne, partTwo);
