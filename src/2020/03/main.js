const runner = require('../../utils/runner');
const filterMap = require('../../utils/filterMap');

const parse = (input) => {
  const lines = input.split('\n');
  return filterMap(lines, (line) => {
    if (line.length === 0) return null;
    return line.split('');
  });
};

const move = (grid, curr, delta) => {
  const width = grid[0].length;
  const nextY = curr[0] + delta[0];
  const nextX = (curr[1] + delta[1]) % width;
  return [nextY, nextX];
};

const isTree = (grid, [x, y]) => {
  try {
    return grid[x][y] === '#';
  } catch {
    return false;
  }
};

const runWithDelta = (delta) => (grid) => {
  let count = 0;
  let position = [0, 0];
  const height = grid.length;
  while (position[0] < height) {
    position = move(grid, position, delta);

    if (isTree(grid, position)) count++;
  }

  return count;
};

const partOne = runWithDelta([1, 3]);
const partTwo = (grid) =>
  [
    [1, 1],
    [1, 3],
    [1, 5],
    [1, 7],
    [2, 1],
  ].reduce((acc, delta) => acc * runWithDelta(delta)(grid), 1);

const main = () => runner(parse, partOne, partTwo);

main();
