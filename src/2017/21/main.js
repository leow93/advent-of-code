const runner = require('../../utils/runner');
const filterMap = require('../../utils/filterMap');

const initialState = `.#.
..#
###`
  .split('\n')
  .map((x) => x.split(''));

const rotate = (matrix) => {
  const rows = matrix.length;
  const cols = matrix[0].length;

  // Create a new matrix with flipped dimensions
  const rotated = Array.from({ length: cols }, () => Array(rows).fill(0));

  // Fill the new matrix with rotated elements
  for (let r = 0; r < rows; r++) {
    for (let c = 0; c < cols; c++) {
      rotated[c][rows - 1 - r] = matrix[r][c];
    }
  }

  return rotated;
};

const rotateN = (matrix, n) => {
  if (n === 0) return matrix;
  if (n === 1) return rotate(matrix);

  return Array.from({ length: n }).reduce((result) => rotate(result), matrix);
};

const flipv = (matrix) => {
  return [...matrix].reverse();
};

const fliph = (matrix) => {
  return matrix.map((row) => [...row].reverse());
};

const size = (arr) => arr.length;

const cache = new Map();
const toString = (matrix) => matrix.map((row) => row.join(',')).join('/');
const key = (pattern, rule) => toString(pattern) + ':' + toString(rule);

const exactMatch = (pattern, rule) => {
  const k = key(pattern, rule);
  if (cache.has(k)) return cache.get(k);
  for (let i = 0; i < pattern.length; i++) {
    for (let j = 0; j < pattern[i].length; j++) {
      if (pattern[i][j] !== rule[i][j]) {
        cache.set(k, false);
        return false;
      }
    }
  }
  cache.set(k, true);
  return true;
};

const match = (pattern, rule) => {
  const k = key(pattern, rule);
  if (cache.has(k)) return cache.get(k);
  const matches = exactMatch(pattern, rule);
  if (matches) {
    return true;
  }

  return [rotateN(matrix, 1), flipv(matrix), fliph(matrix)].some((p) =>
    match(p, rule)
  );
};

const patternMatches = (pattern, rule) => {
  if (size(rule) !== size(pattern)) return false;

  return match(pattern, rule);
};

const genRuleMatcher = (input, output) => {
  const inputGrid = input.split('/').map((x) => x.split(''));
  const outputGrid = output.split('/').map((x) => x.split(''));

  return (pattern) => {
    const match = patternMatches(pattern, inputGrid);
    return {
      match,
      output: outputGrid,
    };
  };
};

const parse = (input) => {
  return filterMap(input.split('\n'), (line) => {
    if (!line || line === '\n') return null;

    const [input, output] = line.split(' => ');

    return genRuleMatcher(input, output);
  });
};

const breakIntoTwos = (grid) => {
  const result = [];
  for (let i = 0; i < grid.length; i += 2) {
    for (let j = 0; j < grid[i].length; j += 2) {
      const matrix = [grid[i].slice(j, j + 2), grid[i + 1].slice(j, j + 2)];
      result.push(matrix);
    }
  }

  return result;
};

const breakIntoThrees = (grid) => {
  const result = [];
  for (let i = 0; i < grid.length; i += 3) {
    for (let j = 0; j < grid[i].length; j += 3) {
      const matrix = [
        grid[i].slice(j, j + 3),
        grid[i + 1].slice(j, j + 3),
        grid[i + 2].slice(j, j + 3),
      ];
      result.push(matrix);
    }
  }

  return result;
};

const join = (subgrids) => {
  const length = subgrids.length;
  const size = Math.sqrt(length);
  const queue = subgrids.slice().map((x) => x.slice());
  const result = [];
  let row = [];
};

const partOne = (data) => {
  console.log({
    initialState,
    data,
  });

  let grid = data;

  while (true) {
    const s = size(grid);
    if (s % 2 === 0) {
      const grids = breakIntoTwos(grid);
      grid = join(
        grids.map((g) => {
          const result = data.find((rule) => rule(g).match);
          return result ? result(g).output : g;
        })
      );
    } else {
    }
  }
};
const partTwo = (data) => 'todo';

runner(parse, partOne, partTwo);
