const { get, movers } = require('./shared');

const parseGrid = (grid) => {
  const rows = grid.split('\n');
  let initialPosition = null;
  const result = [];
  for (let i = 0; i < rows.length; i++) {
    const row = [];
    const chars = rows[i];
    for (let j = 0; j < rows[i].length; j++) {
      const ch = chars[j];
      if (ch === '@') {
        initialPosition = { x: j, y: i };
      }
      row.push(ch === '@' ? '.' : ch);
    }
    result.push(row);
  }

  if (!initialPosition) throw new Error('robot not found');

  return {
    grid: result,
    position: initialPosition,
  };
};

const parse = (input) => {
  const [grid, instructions] = input.split('\n\n');

  return {
    ...parseGrid(grid),
    instructions: instructions.split(''),
  };
};

const distanceToWall = (grid, coord, direction) => {
  const f = movers[direction];
  let d = 0;
  let curr = coord;
  while (curr) {
    const next = f(curr);
    const value = get(grid, next);
    if (!value || value === '#') break;
    d++;
    curr = next;
  }
  return d;
};

const getBoxesToPush = (grid, coord, direction) => {
  const f = movers[direction];
  const result = [];
  let curr = coord;
  while (curr) {
    const next = f(curr);
    const value = get(grid, next);
    if (!value || value !== 'O') break;
    result.push(next);
    curr = next;
  }

  return result;
};

const move = (state, direction) => {
  const mover = movers[direction];
  const nextPosition = mover(state.position);
  const square = get(state.grid, nextPosition);

  if (!square || square === '#') return state;

  if (square === '.') {
    return {
      position: nextPosition,
      grid: state.grid,
    };
  }
  const boxesToPush = getBoxesToPush(state.grid, state.position, direction);
  const distance = distanceToWall(state.grid, state.position, direction);
  if (boxesToPush.length === distance) return state;

  const newGrid = state.grid.slice().map((x) => x.slice());

  for (let i = boxesToPush.length - 1; i >= 0; i--) {
    const brick = boxesToPush[i];
    const curr = get(newGrid, brick);
    const next = mover(brick);
    newGrid[brick.y][brick.x] = '.';
    newGrid[next.y][next.x] = curr;
  }

  return {
    position: nextPosition,
    grid: newGrid,
  };
};

const reducer = (state, instruction) => {
  switch (instruction) {
    case '<':
    case '>':
    case 'v':
    case '^':
      return move(state, instruction);
    default:
      return state;
  }
};

const sumBoxGps = (state) => {
  let result = 0;
  for (let y = 0; y < state.grid.length; y++) {
    for (let x = 0; x < state.grid[0].length; x++) {
      if (state.grid[y][x] !== 'O') continue;
      const gps = x + 100 * y;
      result += gps;
    }
  }
  return result;
};

const partOne = (data) => {
  const state = data.instructions.reduce(reducer, data);
  return sumBoxGps(state);
};

module.exports = (input) => partOne(parse(input));
