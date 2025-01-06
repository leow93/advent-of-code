const runner = require('../../utils/runner');

const parse = (input) => {
  const lines = input.split('\n');
  const grid = [];
  let coord = [0, 0];
  for (let i = 0; i < lines.length; i++) {
    const line = lines[i];
    const row = [];
    for (let j = 0; j < line.length; j++) {
      const ch = line[j];
      if (ch === '^') {
        row.push('.');
        coord = [i, j];
      } else {
        row.push(ch);
      }
    }
    grid.push(row);
  }

  return {
    grid,
    position: { coord: coord, direction: 'N' },
  };
};

const setGridValue = (grid, coord, value) => {
  return grid.map((row, i) =>
    row.map((x, j) => {
      if (i === coord[0] && j === coord[1]) {
        return value;
      }
      return x;
    })
  );
};

class CoordSet {
  data = new Set();

  serialise(coord) {
    return `${coord[0]}:${coord[1]}`;
  }

  add(coord) {
    const key = this.serialise(coord);
    this.data.add(key);
  }

  size() {
    return this.data.size;
  }
}

class PositionSet {
  data = new Set();
  serialise(p) {
    return `${p.direction}:${p.coord[0]}:${p.coord[1]}`;
  }
  add(position) {
    this.data.add(this.serialise(position));
  }
  has(position) {
    return this.data.has(this.serialise(position));
  }
}

const handleNorth = (grid, position) => {
  const [i, j] = position.coord;
  const next = grid[i - 1]?.[j];
  if (!next) {
    return null;
  }
  if (next === '#') {
    return {
      coord: [i, j],
      direction: 'E',
    };
  }

  return {
    coord: [i - 1, j],
    direction: 'N',
  };
};

const handleEast = (grid, position) => {
  const [i, j] = position.coord;
  const next = grid[i]?.[j + 1];
  if (!next) {
    return null;
  }
  if (next === '#') {
    return {
      coord: [i, j],
      direction: 'S',
    };
  }

  return {
    coord: [i, j + 1],
    direction: 'E',
  };
};

const handleSouth = (grid, position) => {
  const [i, j] = position.coord;
  const next = grid[i + 1]?.[j];
  if (!next) {
    return null;
  }
  if (next === '#') {
    return {
      coord: [i, j],
      direction: 'W',
    };
  }

  return {
    coord: [i + 1, j],
    direction: 'S',
  };
};

const handleWest = (grid, position) => {
  const [i, j] = position.coord;
  const next = grid[i]?.[j - 1];
  if (!next) {
    return null;
  }
  if (next === '#') {
    return {
      coord: [i, j],
      direction: 'N',
    };
  }

  return {
    coord: [i, j - 1],
    direction: 'W',
  };
};

const next = (grid, position) => {
  switch (position.direction) {
    case 'N':
      return handleNorth(grid, position);
    case 'E':
      return handleEast(grid, position);
    case 'S':
      return handleSouth(grid, position);
    case 'W':
      return handleWest(grid, position);
    default:
      return null;
  }
};

const walkToExit = (data) => {
  const grid = data.grid;
  let position = data.position;
  const set = new CoordSet();

  while (true) {
    set.add(position.coord);
    position = next(grid, position);
    if (position == null) {
      return set.size();
    }
  }
};

const detectLoop = (data) => {
  const grid = data.grid;
  let position = data.position;
  const set = new PositionSet();

  while (true) {
    if (set.has(position)) {
      return true;
    }
    set.add(position);
    position = next(grid, position);
    if (position == null) {
      return false;
    }
  }
};

const partOne = walkToExit;

const partTwo = (data) => {
  let count = 0;
  const start = data.position;
  const grid = data.grid;

  for (let i = 0; i < grid.length; i++) {
    const row = grid[i];
    for (let j = 0; j < row.length; j++) {
      const x = row[j];
      const isStart = i === start.coord[0] && j === start.coord[1];
      if (x === '#' || isStart) {
        continue;
      }
      if (
        detectLoop({
          grid: setGridValue(grid, [i, j], '#'),
          position: start,
        })
      ) {
        count++;
      }
    }
  }

  return count;
};

const main = () => runner(parse, partOne, partTwo);

main();
