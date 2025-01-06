const next = (grid, curr) => {
  const [i, j] = curr;
  const currValue = grid[i][j];
  return [
    [i + 1, j],
    [i - 1, j],
    [i, j + 1],
    [i, j - 1],
  ].filter(([x, y]) => {
    if (x < 0 || y < 0) return false;
    if (x >= grid.length || y >= grid[0].length) return false;

    return grid[x][y] - 1 === currValue;
  });
};

const dfs = (grid, start, search, cb) => {
  const s = new Stack();
  s.push(start);

  while (s.length() > 0) {
    const curr = s.pop();
    console.log(curr);
    cb(curr);
    if (grid[curr[0]][curr[1]] === search) {
      continue;
    }
    const neighbours = next(grid, curr);
    for (const n of neighbours) {
      s.push(n);
    }
  }
};
class Stack {
  constructor() {
    this.data = [];
  }
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

class Queue {
  constructor() {
    this.data = [];
  }
  enqueue(x) {
    this.data.push(x);
  }
  dequeue() {
    return this.data.shift();
  }
  length() {
    return this.data.length;
  }
}

const bfs = (grid, start, search, cb) => {
  const queue = new Queue();
  queue.enqueue(start);
  while (queue.length() > 0) {
    const curr = queue.dequeue();
    cb(curr);
    if (grid[curr[0]][curr[1]] === search) {
      continue;
    }
    const neighbours = next(grid, curr);
    for (const n of neighbours) {
      queue.enqueue(n);
    }
  }
};

it('does a dfs', () => {
  const grid = [
    [0, 1, 2, 3],
    ['.', 2, '.', '.'],
    ['.', 3, '.', '.'],
  ];

  const order = [];
  dfs(grid, [0, 0], 3, (coord) => order.push(coord));

  expect(order).toEqual([
    [0, 0],
    [0, 1],
    [0, 2],
    [0, 3],
    [1, 1],
    [2, 1],
  ]);
});

it('does a bfs', () => {
  const grid = [
    [0, 1, 2, 3],
    ['.', 2, '.', '.'],
    ['.', 3, '.', '.'],
  ];

  const order = [];
  bfs(grid, [0, 0], 3, (coord) => order.push(coord));
  console.log(order);
  expect(order).toEqual([
    [0, 0],
    [0, 1],
    [1, 1],
    [0, 2],
    [2, 1],
    [0, 3],
  ]);
});
