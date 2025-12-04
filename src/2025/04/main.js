const runner = require('../../utils/runner');
const filterMap = require('../../utils/filterMap');

const parse = (x) => x.split('\n').map((line) => line.split(''));

const directions = [
  [-1, -1],
  [-1, 0],
  [-1, 1],

  [0, -1],
  [0, 1],

  [1, -1],
  [1, 0],
  [1, 1],
];

const neighbours = (grid, i, j) => {
  return filterMap(directions, (d) => {
    const [x, y] = [d[0] + i, d[1] + j];
    if (x >= 0 && x < grid.length && y >= 0 && y < grid[x].length) {
      return [x, y];
    }
    return null;
  });
};

const getRemovable = (grid) => {
  const result = [];
  for (let i = 0; i < grid.length; i++) {
    const row = grid[i];
    for (j = 0; j < row.length; j++) {
      const x = row[j];
      if (x !== '@') continue;
      const ns = neighbours(grid, i, j).filter(([x, y]) => grid[x][y] === '@');
      if (ns.length < 4) result.push([i, j]);
    }
  }
  return result;
};

const partOne = (grid) => getRemovable(grid).length;
const partTwo = (grid) => {
  let g = grid.slice().map((x) => x.slice());
  let count = 0;

  while (true) {
    const toRemove = getRemovable(g);
    if (toRemove.length === 0) {
      return count;
    }

    for (const [x, y] of toRemove) {
      g[x][y] = '.';
      count++;
    }
  }
};

runner(parse, partOne, partTwo);
