const filterMap = require('../../utils/filterMap');
const runner = require('../../utils/runner');

const parse = (input) =>
  filterMap(input.split('\n'), (line) =>
    line == '\n' || !line ? null : line.split('')
  );

const deltas = [
  [-1, -1],
  [-1, 0],
  [-1, 1],
  [0, 1],
  [1, 1],
  [1, 0],
  [1, -1],
  [0, -1],
];

const coordExists = (grid, [x, y]) =>
  x >= 0 && x < grid.length && y >= 0 && y < grid[0].length;

const addCoords = (a, b) => [a[0] + b[0], a[1] + b[1]];

const adjacentSeats = (grid, coord) =>
  deltas.map((x) => addCoords(x, coord)).filter((c) => coordExists(grid, c));

const visibleSeats = (grid, coord) => {
  const seats = [];
  for (const d of deltas) {
    let curr = coord;
    while (true) {
      curr = addCoords(curr, d);
      const cell = grid[curr[0]]?.[curr[1]];
      if (!cell) break;
      if (cell !== '.') {
        seats.push(curr);
        break;
      }
    }
  }

  return seats;
};

const getChanges =
  (getVisibleSeats, neighbourThreshold) =>
  (grid, [i, j]) => {
    const cell = grid[i][j];
    if (cell === '.') return [];

    const neighbours = getVisibleSeats(grid, [i, j]);

    if (cell === 'L') {
      return neighbours.every((coord) => grid[coord[0]][coord[1]] !== '#')
        ? [{ change: '#', coord: [i, j] }]
        : [];
    }

    if (cell === '#') {
      return neighbours.filter(([x, y]) => grid[x][y] === '#').length >=
        neighbourThreshold
        ? [{ change: 'L', coord: [i, j] }]
        : [];
    }

    return [];
  };

const simulate = (grid, getChanges) => {
  const changes = [];

  for (let i = 0; i < grid.length; i++) {
    for (let j = 0; j < grid[i].length; j++) {
      changes.push(...getChanges(grid, [i, j]));
    }
  }
  return changes;
};

const run = (changes) => (input) => {
  const grid = input.slice().map((x) => x.slice());
  let run = true;

  while (run) {
    const changeset = simulate(grid, changes);

    for (const {
      coord: [x, y],
      change,
    } of changeset) {
      grid[x][y] = change;
    }

    run = changeset.length > 0;
  }

  let occupiedSeats = 0;
  for (let i = 0; i < grid.length; i++) {
    const row = grid[i];
    for (let j = 0; j < row.length; j++) {
      const cell = row[j];
      if (cell === '#') occupiedSeats++;
    }
  }

  return occupiedSeats;
};

const partOne = run(getChanges(adjacentSeats, 4));

const partTwo = run(getChanges(visibleSeats, 5));

if (require.main === module) runner(parse, partOne, partTwo);
