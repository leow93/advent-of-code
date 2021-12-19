const fs = require('fs');
const path = require('path');

const getInput = (filename) =>
  fs.readFileSync(path.resolve(__dirname, filename)).toString();

const parseInput = (input) =>
  input
    .split('\n')
    .map((line) =>
      line.split(' -> ').map((coords) => coords.split(',').map(Number))
    );

/**
 *
 * @param input [[[0,0], [0,5]]]
 */
function sizeOfGrid(input) {
  let x = 0,
    y = 0;
  for (let i = 0; i < input.length; i++) {
    const [coord1, coord2] = input[i];
    x = Math.max(coord1[0], coord2[0], x);
    y = Math.max(coord1[1], coord2[1], y);
  }
  return [x, y];
}

function partOne(filename) {
  const input = getInput(filename);
  const parsedInput = parseInput(input);
  const [maxX, maxY] = sizeOfGrid(parsedInput);

  const grid = Array.from({ length: maxX + 1 }).map(() =>
    new Array(maxY + 1).fill(0)
  );

  let counter = 0;
  for (const line of parsedInput) {
    const [[x1, y1], [x2, y2]] = line;
    const startX = Math.min(x1, x2);
    const endX = Math.max(x1, x2);
    const startY = Math.min(y1, y2);
    const endY = Math.max(y1, y2);
    if (x1 !== x2 && y1 !== y2) {
      // diagonal
      const length = endY - startY;
      const xOperation = x1 > x2 ? 'dec' : 'asc';
    } else {
      for (let x = startX; x <= endX; x++) {
        for (let y = startY; y <= endY; y++) {
          grid[x][y]++;
          if (grid[x][y] === 2) {
            counter++;
          }
        }
      }
    }
  }

  return counter;
}

const plus = (a, b) => a + b;
const minus = (a, b) => a - b;

function partTwo(filename) {
  const input = getInput(filename);
  const parsedInput = parseInput(input);
  const [maxX, maxY] = sizeOfGrid(parsedInput);

  const grid = Array.from({ length: maxX + 1 }).map(() =>
    new Array(maxY + 1).fill(0)
  );

  let counter = 0;
  for (const line of parsedInput) {
    const [[x1, y1], [x2, y2]] = line;
    const startX = Math.min(x1, x2);
    const endX = Math.max(x1, x2);
    const startY = Math.min(y1, y2);
    const endY = Math.max(y1, y2);
    if (x1 !== x2 && y1 !== y2) {
      // diagonal
      const length = endY - startY;
      const xOperation = x1 > x2 ? minus : plus;
      const yOperation = y1 > y2 ? minus : plus;
      for (let i = 0; i <= length; i++) {
        const [x, y] = [xOperation(x1, i), yOperation(y1, i)];
        grid[x][y]++;
        if (grid[x][y] === 2) {
          counter++;
        }
      }
    } else {
      for (let x = startX; x <= endX; x++) {
        for (let y = startY; y <= endY; y++) {
          grid[x][y]++;
          if (grid[x][y] === 2) {
            counter++;
          }
        }
      }
    }
  }
  // draw(grid);

  return counter;
}

console.log('Part I test:', partOne('test.txt'));
console.log('Part I:', partOne('data.txt'));

console.log('Part II test:', partTwo('test.txt'));
console.log('Part II:', partTwo('data.txt'));

function draw(grid) {
  for (const line of grid) {
    for (const value of line) {
      process.stdout.write(value === 0 ? '.' : value.toString());
    }
    process.stdout.write('\n');
  }
}
