const runner = require('../../utils/runner');

class Grid {
  #state = new Map();

  key(row, col) {
    return `${row},${col}`;
  }

  state(row, col) {
    if (this.isInfected(row, col)) {
      return '#';
    }

    if (this.isWeakened(row, col)) {
      return 'W';
    }

    if (this.isFlagged(row, col)) {
      return 'F';
    }

    return '.';
  }

  isFlagged(row, col) {
    return this.#state.get(this.key(row, col)) === 'F';
  }

  isWeakened(row, col) {
    return this.#state.get(this.key(row, col)) === 'W';
  }

  isInfected(row, col) {
    return this.#state.get(this.key(row, col)) === '#';
  }

  isClean(row, col) {
    return this.#state.get(this.key(row, col)) == undefined;
  }

  infect(row, col) {
    this.#state.set(this.key(row, col), '#');
  }

  clean(row, col) {
    this.#state.delete(this.key(row, col));
  }

  flag(row, col) {
    this.#state.set(this.key(row, col), 'F');
  }
  weaken(row, col) {
    this.#state.set(this.key(row, col), 'W');
  }
}

const parse = (input) => {
  const lines = input.split('\n').filter((x) => x !== '');
  const infected = [];
  for (let i = 0; i < lines.length; i++) {
    const line = lines[i];
    for (let j = 0; j < line.length; j++) {
      if (line[j] === '#') infected.push([i, j]);
    }
  }
  const start = Math.floor(lines.length / 2);

  return {
    infected,
    initialState: {
      row: start,
      col: start,
      direction: 'u',
    },
  };
};

const turnRight = (d) => {
  const m = { u: 'r', r: 'd', d: 'l', l: 'u' };
  if (!(d in m)) {
    throw new Error();
  }
  return m[d];
};

const turnLeft = (d) => {
  const m = { u: 'l', r: 'u', d: 'r', l: 'd' };
  if (!(d in m)) {
    throw new Error();
  }
  return m[d];
};

const turnAround = (d) => {
  const m = { u: 'd', r: 'l', d: 'u', l: 'r' };
  if (!(d in m)) {
    throw new Error();
  }
  return m[d];
};

const moveForward = (row, col, d) => {
  switch (d) {
    case 'u':
      return [row - 1, col];
    case 'r':
      return [row, col + 1];
    case 'd':
      return [row + 1, col];
    case 'l':
      return [row, col - 1];
    default:
      throw new Error();
  }
};

const partOne = (x) => {
  let curr = { ...x.initialState };
  const grid = new Grid();
  for (const [r, c] of x.infected) {
    grid.infect(r, c);
  }

  let causingInfection = 0;
  const MAX = 10_000;
  let i = 0;
  while (i < MAX) {
    if (grid.isInfected(curr.row, curr.col)) {
      curr.direction = turnRight(curr.direction);
      grid.clean(curr.row, curr.col);
    } else {
      curr.direction = turnLeft(curr.direction);
      causingInfection++;
      grid.infect(curr.row, curr.col);
    }

    const [nextRow, nextCol] = moveForward(curr.row, curr.col, curr.direction);
    curr = {
      direction: curr.direction,
      row: nextRow,
      col: nextCol,
    };

    i++;
  }

  return causingInfection;
};

const partTwo = (x) => {
  let curr = { ...x.initialState };
  const grid = new Grid();
  for (const [r, c] of x.infected) {
    grid.infect(r, c);
  }
  let causingInfection = 0;
  const MAX = 10000000;
  let i = 0;
  while (i < MAX) {
    let nextDirection = curr.direction;
    switch (grid.state(curr.row, curr.col)) {
      // clean
      case '.': {
        nextDirection = turnLeft(curr.direction);
        grid.weaken(curr.row, curr.col);
        break;
      }
      // weakened
      case 'W':
        grid.infect(curr.row, curr.col);
        causingInfection++;
        break;
      // infected
      case '#':
        nextDirection = turnRight(curr.direction);
        grid.flag(curr.row, curr.col);
        break;
      // flagged
      case 'F':
        nextDirection = turnAround(curr.direction);
        grid.clean(curr.row, curr.col);
        break;
      default:
        throw new Error('unexpected state');
    }

    const [nextRow, nextCol] = moveForward(curr.row, curr.col, nextDirection);
    curr = {
      direction: nextDirection,
      row: nextRow,
      col: nextCol,
    };

    i++;
  }

  return causingInfection;
};

runner(parse, partOne, partTwo);
