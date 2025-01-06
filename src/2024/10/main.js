const filterMap = require('../../utils/filterMap');
const runner = require('../../utils/runner');
const parse = (input) =>
  filterMap(input.split('\n'), (line) => {
    if (!line) return null;
    return line.split('').map((x) => {
      const p = parseInt(x);
      if (isNaN(p)) {
        return '.';
      }
      return p;
    });
  });

const next = (grid, curr) => {
  const [i, j] = curr;
  const currValue = grid[i][j];
  return [
    [i + 1, j],
    [i - 1, j],
    [i, j + 1],
    [i, j - 1],
  ].filter(([x, y]) => {
    if (x < 0 || y < 0) return false;
    if (x >= grid.length || y >= grid[0].length) return false;

    return grid[x][y] - 1 === currValue;
  });
};

const dfs = (grid, start, search, cb) => {
  const queue = [start];
  while (queue.length) {
    const curr = queue.shift();
    if (grid[curr[0]][curr[1]] === search) {
      cb(curr);
      continue;
    }
    const neighbours = next(grid, curr);
    for (const n of neighbours) {
      queue.unshift(n);
    }
  }
};

const countTrailends = (grid, start, search) => {
  const trailheads = new Set();
  dfs(grid, start, search, ([i, j]) => trailheads.add(`${i},${j}`));
  return trailheads.size;
};

const trailRating = (grid, start, search) => {
  let count = 0;
  dfs(grid, start, search, () => count++);
  return count;
};

const run = (counter) => (grid) => {
  let count = 0;
  for (let i = 0; i < grid.length; i++) {
    const row = grid[i];
    for (let j = 0; j < row.length; j++) {
      const cell = row[j];
      if (cell === 0) {
        count += counter(grid, [i, j], 9);
      }
    }
  }

  return count;
};

const partOne = run(countTrailends);
const partTwo = run(trailRating);

runner(parse, partOne, partTwo);
