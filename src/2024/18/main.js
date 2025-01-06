const runner = require('../../utils/runner');
const filterMap = require('../../utils/filterMap');

const parse = (input) => {
  let max = 0;
  const coords = filterMap(input.split('\n'), (line) => {
    if (!line || line === '\n') return null;

    const [x, y] = line.split(',').map(Number);

    if (x > max) max = x;
    if (y > max) max = y;

    return { x, y };
  });

  const grid = Array.from({ length: max + 1 }).map(() =>
    Array.from({ length: max + 1 }).fill('.')
  );

  return {
    coords,
    grid,
  };
};

const printGrid = (grid) => {
  let out = '';
  for (const line of grid) {
    out += line.join('') + '\n';
  }

  console.log(out);
};

const markCorruputed = (grid, coord) =>
  grid.map((row, y) =>
    row.map((cell, x) => (coord.x === x && coord.y === y ? '#' : cell))
  );

const directions = [
  { dx: 0, dy: -1 }, // North
  { dx: 1, dy: 0 }, // East
  { dx: 0, dy: 1 }, // South
  { dx: -1, dy: 0 }, // West
];

const shortestPath = (grid, start, end) => {
  const isValid = ({ x, y }) => {
    return (
      y >= 0 &&
      y < grid.length &&
      x >= 0 &&
      x < grid[0].length &&
      grid[y][x] !== '#'
    );
  };

  const visited = new Set();
  const queue = [{ x: start.x, y: start.y, steps: 0 }];
  let score = Infinity;

  while (queue.length > 0) {
    const { x, y, steps } = queue.shift();

    const id = `${x},${y}`;

    if (visited.has(id)) continue;
    visited.add(id);

    if (x === end.x && y === end.y) {
      if (steps < score) {
        score = steps;
      }
      continue;
    }

    for (const d of directions) {
      const next = { x: x + d.dx, y: y + d.dy };
      if (isValid(next)) {
        queue.push({ x: next.x, y: next.y, steps: steps + 1 });
      }
    }
  }

  return score;
};

const reduceGrid = (data, n) => {
  return data.coords.slice(0, n).reduce(markCorruputed, data.grid);
};
const getShortestPath = (grid, n) => {
  const start = { x: 0, y: 0 };
  const end = { y: grid.length - 1, x: grid[0].length - 1 };
  return shortestPath(grid, start, end);
};

const runNInstructions = (data, n) => {
  const grid = reduceGrid(data, n);
  return getShortestPath(grid, n);
};

const partOne = (data) => runNInstructions(data, 1024);
const partTwo = (data) => {
  for (let i = 1024; i < data.coords.length; i++) {
    console.log(`${i} / ${data.coords.length - 1}`);
    const result = runNInstructions(data, i);
    if (result === Infinity) {
      const coord = data.coords[i - 1];
      return `${coord.x},${coord.y}`;
    }
  }
  throw new Error('not found');
};

runner(parse, partOne, partTwo);
