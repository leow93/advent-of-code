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

const withinGrid = (grid, coord) => {
  return (
    coord[0] >= 0 &&
    coord[0] < grid.length &&
    coord[1] >= 0 &&
    coord[1] < grid[0].length
  );
};

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

const partOne = walkToExit;

const partTwo = (data) => 0;

const main = () => runner(parse, partOne, partTwo);

main();
