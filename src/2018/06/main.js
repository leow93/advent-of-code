const runner = require('../../utils/runner');

const parse = (input) => {
  const lines = input.split('\n');
  const coords = [];
  let maxX = 0;
  let maxY = 0;
  for (const l of lines) {
    if (!l || l === '\n') continue;
    const [x, y] = l.split(', ').map(Number);
    coords.push([y, x]);
    if (y > maxY) maxY = y;
    if (x > maxX) maxX = x;
  }
  return {
    coords,
    maxX: maxX + 1,
    maxY: maxY + 1,
  };
};

const coordName = (idx) => String.fromCharCode('A'.charCodeAt(0) + idx);

const manhattan = (a, b) => Math.abs(b[0] - a[0]) + Math.abs(b[1] - a[1]);

const print = (grid) => {
  for (let i = 0; i < grid.length; i++) {
    console.log(grid[i].join(''));
  }
};

class Stack {
  constructor() {
    this.data = [];
  }
  push(x) {
    this.data.push(x);
  }
  pop() {
    return this.data.pop();
  }
}

const finiteAreas = (grid) => {
  const areas = [];
  const curr = new Stack();
  curr.push({ coords: [], isFinite: true });
  const visited = new Set();

  const queue = [[0, 0]];
  while (queue.length) {
    // take from the front
    const [y, x] = queue.shift();
    const key = `${y},${x}`;
    if (visited.has(key)) continue;
    const area = curr.pop();
    const value = grid[y][x];
    if (!area.value) {
      area.value = value;
    } else if (area.value !== value) {
      // put to the back of queue
      queue.push([y, x]);
      continue;
    }

    area.coords.push([y, x]);
    visited.add(key);
  }
};

const partOne = ({ coords, maxX, maxY }) => {
  const grid = Array.from({ length: maxY + 1 }).map(() =>
    Array.from({ length: maxX + 1 }).fill('.')
  );
  for (let i = 0; i < maxY + 1; i++) {
    for (let j = 0; j < maxX + 1; j++) {
      const distances = coords
        .map((coord, k) => ({
          coord,
          distance: manhattan(coord, [i, j]),
          id: coordName(k),
        }))
        .sort((a, b) => a.distance - b.distance);
      const [a, b] = distances;

      if (a.distance === b.distance) {
        grid[i][j] = '.';
      } else if (a.coord[0] === i && a.coord[1] === j) {
        grid[i][j] = a.id;
      } else {
        grid[i][j] = a.id.toLowerCase();
      }
    }
  }

  print(grid);

  return null;
};

const partTwo = (polymer) => {
  return null;
};

runner(parse, partOne, partTwo);
