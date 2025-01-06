const parseGrid = (grid) => {
  const rows = grid.split('\n');
  let initialPosition = null;
  const result = [];
  for (let i = 0; i < rows.length; i++) {
    const chars = Array.from(rows[i]);

    const row = chars.flatMap((x, j) => {
      if (x === '#' || x === '.') return [x, x];
      if (x === 'O') {
        return ['[', ']'];
      }

      if (x === '@') {
        return ['@', '.'];
      }
      return [];
    });

    const j = row.findIndex((x) => x === '@');
    if (j >= 0) {
      row[j] = '.';
      initialPosition = { y: i, x: j };
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

const boxesToPushLeft = ({ grid, position }) => {
  const boxes = [];
  let curr = position;
  while (true) {
    const next = { y: curr.y, x: curr.x - 1 };
    const elm = grid[next.y][next.x];
    if (!elm || elm === '#') {
      return [];
    }
    if (elm === '.') {
      return boxes;
    }
    if (elm === '[') {
      boxes.unshift(next);
    }
    curr = next;
  }
};

const boxesToPushRight = ({ grid, position }) => {
  const boxes = [];
  let curr = position;
  while (true) {
    const next = { y: curr.y, x: curr.x + 1 };
    const elm = grid[next.y][next.x];
    if (!elm || elm === '#') {
      return [];
    }
    if (elm === '.') {
      return boxes;
    }
    if (elm === '[') {
      boxes.unshift(next);
    }
    curr = next;
  }
};

const slice = (xs) => xs.slice();

const moveLeft = (state) => {
  const { grid, position } = state;
  const next = grid[position.y][position.x - 1];
  if (!next) return state;

  if (next !== ']' && next !== '.') return state;

  const newPosition = { y: position.y, x: position.x - 1 };
  if (next === '.') {
    const newGrid = slice(grid).map(slice);
    return { grid: newGrid, position: newPosition };
  }

  const boxesToPush = boxesToPushLeft(state);
  if (boxesToPush.length === 0) return state;

  const newGrid = slice(grid).map(slice);

  for (const b of boxesToPush) {
    const value = newGrid[b.y][b.x];
    if (value === '[') {
      // set new
      newGrid[b.y][b.x - 1] = '[';
      newGrid[b.y][b.x] = ']';
      newGrid[b.y][b.x + 1] = '.';
    }
  }

  return {
    grid: newGrid,
    position: newPosition,
  };
};

const moveRight = (state) => {
  const { grid, position } = state;
  const next = grid[position.y][position.x + 1];
  if (!next) return state;

  if (next !== '[' && next !== '.') return state;

  const newPosition = { y: position.y, x: position.x + 1 };
  if (next === '.') {
    const newGrid = slice(grid).map(slice);
    return { grid: newGrid, position: newPosition };
  }

  const boxesToPush = boxesToPushRight(state);
  if (boxesToPush.length === 0) return state;

  const newGrid = slice(grid).map(slice);

  for (const b of boxesToPush) {
    const value = newGrid[b.y][b.x];
    if (value === '[') {
      // set new
      newGrid[b.y][b.x + 1] = '[';
      newGrid[b.y][b.x + 2] = ']';
      newGrid[b.y][b.x] = '.';
    }
  }

  return {
    grid: newGrid,
    position: newPosition,
  };
};

class Stack {
  data = [];
  push(x) {
    this.data.push(x);
  }
  pop() {
    return this.data.pop();
  }

  length() {
    return this.data.length;
  }
}

class UniqArray {
  constructor(hashFn) {
    this.hash = hashFn;
    this.data = [];
  }

  unshift(item) {
    const key = this.hash(item);
    if (this.data.map(this.hash).includes(key)) {
      return;
    }
    this.data.unshift(item);
  }
  items() {
    return this.data;
  }

  get length() {
    return this.data.length;
  }
}

const moveDown = (state) => {
  const { grid, position } = state;
  const next = grid[position.y + 1][position.x];
  if (!next) return state;
  if (next === '.') {
    return {
      grid,
      position: { x: position.x, y: position.y + 1 },
    };
  }

  const stack = new Stack();
  stack.push({ x: position.x, y: position.y + 1 });
  const visited = new Set();
  const id = ({ x, y }) => `${x},${y}`;
  const nodesToPush = new UniqArray(id);
  while (stack.length() > 0) {
    const curr = stack.pop();
    if (!curr) break;
    if (visited.has(id(curr))) continue;
    visited.add(id(curr));

    const currValue = grid[curr.y][curr.x];

    if (!currValue || currValue === '#') {
      return state;
    }

    if (currValue === '[') {
      // record node as box to be pushed
      nodesToPush.unshift(curr);
      // push below and below-right
      stack.push({ y: curr.y + 1, x: curr.x });
      stack.push({ y: curr.y + 1, x: curr.x + 1 });
    } else if (currValue === ']') {
      // record left node as box to be pushed
      nodesToPush.unshift({ y: curr.y, x: curr.x - 1 });
      // push below and below-left
      stack.push({ y: curr.y + 1, x: curr.x });
      stack.push({ y: curr.y + 1, x: curr.x - 1 });
    }
  }

  if (nodesToPush.length === 0) return state;
  const nextPosition = { x: position.x, y: position.y + 1 };
  const nextGrid = slice(grid).map(slice);

  for (const left of nodesToPush.items().sort((a, b) => b.y - a.y)) {
    const right = { ...left, x: left.x + 1 };
    // pushing down

    nextGrid[left.y][left.x] = '.';
    nextGrid[right.y][right.x] = '.';
    nextGrid[left.y + 1][left.x] = '[';
    nextGrid[right.y + 1][right.x] = ']';
  }
  return {
    grid: nextGrid,
    position: nextPosition,
  };
};

const moveUp = (state) => {
  const { grid, position } = state;
  const next = grid[position.y - 1][position.x];
  if (!next) return state;
  if (next === '.') {
    return {
      grid,
      position: { x: position.x, y: position.y - 1 },
    };
  }

  const stack = new Stack();
  stack.push({ x: position.x, y: position.y - 1 });
  const visited = new Set();
  const id = ({ x, y }) => `${x},${y}`;
  const nodesToPush = new UniqArray(id);

  while (stack.length() > 0) {
    const curr = stack.pop();
    if (!curr) break;
    if (visited.has(id(curr))) continue;
    visited.add(id(curr));

    const currValue = grid[curr.y][curr.x];

    if (!currValue || currValue === '#') {
      return state;
    }

    if (currValue === '[') {
      // record node as box to be pushed
      nodesToPush.unshift(curr);
      // push above and above-right
      stack.push({ y: curr.y - 1, x: curr.x });
      stack.push({ y: curr.y - 1, x: curr.x + 1 });
    } else if (currValue === ']') {
      // record left node as box to be pushed
      nodesToPush.unshift({ y: curr.y, x: curr.x - 1 });
      // push above and above-left
      stack.push({ y: curr.y - 1, x: curr.x });
      stack.push({ y: curr.y - 1, x: curr.x - 1 });
    }
  }

  if (nodesToPush.length === 0) return state;

  const nextPosition = { x: position.x, y: position.y - 1 };

  const nextGrid = slice(grid).map(slice);

  for (const left of nodesToPush.items().sort((a, b) => a.y - b.y)) {
    const right = { ...left, x: left.x + 1 };
    // pushing up

    nextGrid[left.y][left.x] = '.';
    nextGrid[right.y][right.x] = '.';
    nextGrid[left.y - 1][left.x] = '[';
    nextGrid[right.y - 1][right.x] = ']';
  }
  return {
    grid: nextGrid,
    position: nextPosition,
  };
};

const sanityCheck = ({ grid, position }, instruction, i, previous) => {
  // position cannot occupy a box or wall
  const positionValue = grid[position.y][position.x];
  if (positionValue !== '.') throw new Error('Invalid position.');

  // each box must be valid
  for (let y = 0; y < grid.length; y++) {
    for (let x = 0; x < grid[y].length; x++) {
      const ch = grid[y][x];
      if (ch === '[') {
        const nxt = grid[y][x + 1];
        if (nxt !== ']') {
          console.log('PREVIOUS');
          console.log(format(previous));
          console.log({
            instruction,
            instructionIdx: i,
            y,
            x,
          });
          console.log('RESULT');
          console.log(format({ grid, position }));
          throw new Error('Box not closed properly');
        }
      } else if (ch === ']') {
        const prev = grid[y][x - 1];
        if (prev !== '[') {
          console.log('PREVIOUS');
          console.log(format(previous));
          console.log({
            instruction,
            instructionIdx: i,
            y,
            x,
          });
          console.log('RESULT');
          console.log(format({ grid, position }));

          throw new Error('Box not opened properly');
        }
      }
    }
  }
};

const reducer = (state, instruction, i) => {
  const result = (() => {
    switch (instruction) {
      case '<':
        return moveLeft(state);

      case '>':
        return moveRight(state);

      case 'v':
        return moveDown(state);
      case '^':
        return moveUp(state);

      default:
        return state;
    }
  })();

  sanityCheck(result, instruction, i, state);

  return result;
};

const eq = (a, b) => a.x === b.x && a.y === b.y;

const format = (state) => {
  const rows = [];
  for (let i = 0; i < state.grid.length; i++) {
    let row = '';
    for (let j = 0; j < state.grid[i].length; j++) {
      if (eq(state.position, { x: j, y: i })) {
        row += '@';
        continue;
      }
      row += state.grid[i][j];
    }
    rows.push(row);
  }

  const formatted = rows.join('\n');
  return formatted;
};

const sumGPSCoords = ({ grid }) => {
  let result = 0;
  for (let y = 0; y < grid.length; y++) {
    for (let x = 0; x < grid[y].length; x++) {
      const ch = grid[y][x];
      if (ch !== '[') continue;
      const score = 100 * y + x;
      result += score;
    }
  }

  return result;
};

module.exports = {
  parse,
  reducer,
  format,
  sumGPSCoords,
};
