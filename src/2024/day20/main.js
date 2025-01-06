const runner = require('../../utils/runner');

const parse = (input) => {
  const grid = [];
  let start, end;
  const lines = input.split('\n');

  for (let i = 0; i < lines.length; i++) {
    const line = lines[i];
    if (line === '' || line === '\n') continue;
    const chars = line.split('');
    const row = [];

    for (let j = 0; j < chars.length; j++) {
      const ch = chars[j];
      if (ch === 'S') {
        start = [i, j];
        row.push('.');
        continue;
      }
      if (ch === 'E') {
        end = [i, j];
        row.push('.');
        continue;
      }

      row.push(ch);
    }

    grid.push(row);
  }

  return {
    start,
    end,
    grid,
  };
};

const directions = [
  [-1, 0],
  [0, 1],
  [1, 0],
  [0, -1],
];
const isValid = (grid, [x, y]) =>
  x >= 0 &&
  x < grid.length &&
  y >= 0 &&
  y < grid[0].length &&
  grid[x][y] !== '#';

const defaultRun = (grid, start, end) => {
  const queue = [{ coord: start, length: 0 }];
  const visited = new Set();
  const cheats = new Set();

  let results = [];

  while (queue.length > 0) {
    const curr = queue.shift();
    const { coord, length } = curr;
    const [x, y] = coord;
    if (x === end[0] && y === end[1]) {
      results.push(length);
      continue;
    }

    if (visited.has(`${x},${y}`)) continue;
    visited.add(`${x},${y}`);

    for (const d of directions) {
      const next = [x + d[0], y + d[1]];
      if (isValid(grid, next)) {
        queue.push({ coord: next, length: length + 1 });
      } else {
        // not valid, perhaps we can cheat.
        // cheating means going one square over in the same direction
        const cheat = [x + 2 * d[0], y + 2 * d[1]];
        if (isValid(grid, cheat)) {
          cheats.add(`${next[0]},${next[1]}`);
        }
      }
    }
  }
  return {
    minPath: Math.min(...results),
    cheats,
  };
};

const cheatRun = (originalGrid, start, end, cheat) => {
  const [x, y] = cheat.split(',').map(Number);
  const grid = originalGrid.map((row, i) =>
    row.map((cell, j) => (i === x && j === y ? '.' : cell))
  );

  const queue = [{ coord: start, length: 0 }];
  const visited = new Set();

  let results = [];

  while (queue.length > 0) {
    const curr = queue.shift();
    const { coord, length } = curr;
    const [x, y] = coord;
    if (x === end[0] && y === end[1]) {
      results.push(length);
      continue;
    }

    if (visited.has(`${x},${y}`)) continue;
    visited.add(`${x},${y}`);

    for (const d of directions) {
      const next = [x + d[0], y + d[1]];
      if (isValid(grid, next)) {
        queue.push({ coord: next, length: length + 1 });
      }
    }
  }

  return {
    minPath: Math.min(...results),
  };
};

const partOne = (data) => {
  const { grid, start, end } = data;
  const { minPath, cheats } = defaultRun(grid, start, end);

  const timeSaved = {};

  for (const cheat of cheats) {
    const { minPath: cheatMinPath } = cheatRun(grid, start, end, cheat);
    const saved = minPath - cheatMinPath;

    timeSaved[saved] = timeSaved[saved] ?? 0;
    timeSaved[saved] += 1;
  }

  let cheatsThatSave100PS = 0;
  for (const [k, n] of Object.entries(timeSaved)) {
    if (Number(k) < 100) continue;
    cheatsThatSave100PS += n;
  }

  return cheatsThatSave100PS;
};
const partTwo = (data) => 'todo';

runner(parse, partOne, partTwo);
