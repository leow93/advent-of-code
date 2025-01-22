const runner = require('../../utils/runner');
const filterMap = require('../../utils/filterMap');

const parse = (input) => {
  const lines = input.split('\n');
  let start = null;

  const grid = filterMap(lines, (line, i) => {
    if (!line || line === '\n') return null;
    const chars = line.split('');
    if (i === 0) {
      const x = chars.findIndex((c) => c === '|');
      start = [0, x];
    }

    return chars;
  });

  return {
    position: start,
    direction: 'S',
    grid,
  };
};

const isChar = (x) => x.toLowerCase() !== x.toUpperCase();

const partOne = (data) => {
  const grid = data.grid;
  const queue = [{ position: data.position, direction: data.direction }];
  const result = [];
  const valid = ({ position }) => {
    const [y, x] = position;
    const value = grid[y]?.[x];
    return value !== undefined && value !== '';
  };
  const next = (curr) => {
    const { position, direction } = curr;
    const [y, x] = position;
    const v = grid[y][x];
    switch (direction) {
      case 'S': {
        if (v === '|' || v === '-' || isChar(v)) {
          return [{ direction, position: [y + 1, x] }];
        }

        if (v === '+') {
          return [
            { direction: 'E', position: [y, x + 1] },
            {
              direction: 'W',
              position: [y, x - 1],
            },
          ];
        }
        return [];
      }
      case 'N': {
        if (v === '|' || v === '-' || isChar(v)) {
          return [{ direction, position: [y - 1, x] }];
        }

        if (v === '+') {
          return [
            { direction: 'E', position: [y, x + 1] },
            { direction: 'W', position: [y, x - 1] },
          ];
        }
        return [];
      }

      case 'E': {
        if (v === '-' || v === '|' || isChar(v)) {
          return [{ direction, position: [y, x + 1] }];
        }

        if (v === '+') {
          return [
            { direction: 'N', position: [y - 1, x] },
            { direction: 'S', position: [y + 1, x] },
          ];
        }
        return [];
      }

      case 'W': {
        if (v === '-' || v === '|' || isChar(v)) {
          return [{ direction, position: [y, x - 1] }];
        }

        if (v === '+') {
          return [
            { direction: 'N', position: [y - 1, x] },
            { direction: 'S', position: [y + 1, x] },
          ];
        }
        return [];
      }

      default: {
        return [];
      }
    }
  };

  let steps = 0;

  while (queue.length > 0) {
    const { position, direction } = queue.shift();
    const [y, x] = position;
    const v = grid[y]?.[x];
    if (!v || v === '' || v === ' ') continue;

    if (isChar(v)) result.push(v);

    steps++;

    for (const n of next({ position, direction })) {
      if (valid(n)) {
        queue.push(n);
      }
    }
  }
  return [result.join(''), steps];
};
const partTwo = (data) => 'implicitly done in part one';

runner(parse, partOne, partTwo);
