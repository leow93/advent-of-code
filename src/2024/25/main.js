const runner = require('../../utils/runner');

const parse = (input) => {
  const items = input.split('\n\n');
  const locks = [];
  const keys = [];

  for (const item of items) {
    const matrix = item
      .split('\n')
      .filter((x) => x !== '' && x !== '\n')
      .map((x) => x.split(''));
    const nRows = matrix.length;
    const nCols = matrix[0].length;

    if (matrix[0].every((x) => x === '#')) {
      const heights = [];
      for (let i = 0; i < nCols; i++) {
        let height = 0;
        for (let j = 1; j < nRows; j++) {
          if (matrix[j][i] === '#') height++;
        }
        heights.push(height);
      }
      locks.push(heights);
    } else {
      const heights = [];
      for (let i = 0; i < nCols; i++) {
        let height = 0;
        for (let j = nRows - 2; j >= 0; j--) {
          if (matrix[j][i] === '#') height++;
        }
        heights.push(height);
      }
      keys.push(heights);
    }
  }

  return { locks, keys };
};

const partOne = (data) => {
  const { locks, keys } = data;

  let count = 0;
  for (const lock of locks) {
    for (const key of keys) {
      const valid = key.every((height, i) => height + lock[i] <= 5);
      if (valid) count++;
    }
  }

  return count;
};
const partTwo = (data) => 'todo';

runner(parse, partOne, partTwo);
