const readFromStdin = require('../../utils/readFromStdin');

const parse = (input) => {
  return input.split('\n').map((row) => row.split(''));
};

class MinPriorityQueue {
  constructor({ priority }) {
    this.heap = [];
    this.priority = priority;
  }

  inspect() {
    const itvl = setInterval(
      () => console.log('Queue length: ', this.heap.length),
      1000
    );

    return () => {
      clearInterval(itvl);
    };
  }

  enqueue(item) {
    const heap = this.heap.slice();
    heap.push(item);
    this.heap = heap.sort((a, b) => this.priority(a, b));
  }

  dequeue() {
    if (this.isEmpty()) {
      throw new Error('Queue is empty');
    }
    const item = this.heap.shift();
    return item;
  }

  isEmpty() {
    return this.heap.length === 0;
  }
}

function isValidMove(x, y, map) {
  const numRows = map.length;
  const numCols = map[0].length;

  return x >= 0 && x < numCols && y >= 0 && y < numRows && map[y][x] !== '#';
}

function findStart(map) {
  const numRows = map.length;
  const numCols = map[0].length;

  for (let y = 0; y < numRows; y++) {
    for (let x = 0; x < numCols; x++) {
      if (map[y][x] === 'S') {
        return { x, y };
      }
    }
  }
  throw new Error('Start tile not found');
}

function findEnd(map) {
  const numRows = map.length;
  const numCols = map[0].length;

  for (let y = 0; y < numRows; y++) {
    for (let x = 0; x < numCols; x++) {
      if (map[y][x] === 'E') {
        return { x, y };
      }
    }
  }
  throw new Error('End tile not found');
}

function markBestPaths(map, bestPaths) {
  const numRows = map.length;
  const numCols = map[0].length;

  for (let y = 0; y < numRows; y++) {
    for (let x = 0; x < numCols; x++) {
      const key = `${x},${y}`;
      if (bestPaths.has(key) && map[y][x] === '.') {
        map[y][x] = 'O';
      }
    }
  }
  console.log(map.map((row) => row.join('')).join('\n'));
}

const directions = [
  { dx: 1, dy: 0 }, // East
  { dx: 0, dy: 1 }, // South
  { dx: -1, dy: 0 }, // West
  { dx: 0, dy: -1 }, // North
];

const dir = (idx) => {
  switch (idx) {
    case 0:
      return 'E';
    case 1:
      return 'S';
    case 2:
      return 'W';
    case 3:
      return 'N';
    default:
      return 'none';
  }
};

function partOne(map) {
  const start = findStart(map);
  const end = findEnd(map);

  const pq = new MinPriorityQueue({ priority: (a, b) => a.score - b.score });

  pq.enqueue({ x: start.x, y: start.y, dirIndex: 0, score: 0, path: [] });

  const visited = new Set();
  const scores = [];

  while (!pq.isEmpty()) {
    const { x, y, dirIndex, score, path } = pq.dequeue();
    const key = `${x},${y},${dirIndex}`;

    if (visited.has(key)) continue;
    visited.add(key);

    const newPath = [...path, `${x},${y}`];

    if (x === end.x && y === end.y) {
      scores.push(score);
      continue;
    }

    // Move forward
    const nx = x + directions[dirIndex].dx;
    const ny = y + directions[dirIndex].dy;
    if (isValidMove(nx, ny, map)) {
      pq.enqueue({ x: nx, y: ny, dirIndex, score: score + 1, path: newPath });
    }

    // Rotate clockwise or counterclockwise
    for (let turn = -1; turn <= 1; turn += 2) {
      const newDirIndex = (dirIndex + turn + 4) % 4;
      pq.enqueue({
        x,
        y,
        dirIndex: newDirIndex,
        score: score + 1000,
        path: newPath,
      });
    }
  }

  return Math.min(...scores);
}
function partTwoOld(map, optimalScore) {
  const start = findStart(map);
  const end = findEnd(map);

  const pq = new MinPriorityQueue({ priority: (a, b) => a.score - b.score });
  pq.enqueue({ x: start.x, y: start.y, dirIndex: 0, score: 0, path: [] });

  const visited = new Set();
  const bestPaths = new Set();

  while (!pq.isEmpty()) {
    const { x, y, dirIndex, score, path } = pq.dequeue();
    const key = `${x},${y},${dirIndex}`;

    if (visited.has(key)) continue;
    visited.add(key);

    const newPath = [...path, `${x},${y}`];

    if (x === end.x && y === end.y) {
      if (score === optimalScore) {
        console.log('best', score);
        console.log(newPath);
        newPath.forEach((tile) => bestPaths.add(tile));
      } else {
        console.log('not best', score);
      }
      continue;
    }
    // Move forward
    const nx = x + directions[dirIndex].dx;
    const ny = y + directions[dirIndex].dy;
    if (isValidMove(nx, ny, map)) {
      pq.enqueue({ x: nx, y: ny, dirIndex, score: score + 1, path: newPath });
    }

    // Rotate clockwise or counterclockwise
    for (let turn = -1; turn <= 1; turn += 2) {
      const newDirIndex = (dirIndex + turn + 4) % 4;
      pq.enqueue({
        x,
        y,
        dirIndex: newDirIndex,
        score: score + 1000,
        path: newPath,
      });
    }
  }
  markBestPaths(map, bestPaths);

  return bestPaths.size;
}

async function partTwo(map, optimalScore) {
  const start = findStart(map);
  const end = findEnd(map);

  const pq = new MinPriorityQueue({ priority: (a, b) => a.score - b.score });
  pq.enqueue({
    x: start.x,
    y: start.y,
    dirIndex: 0,
    score: 0,
    path: new Set([`${start.x},${start.y}`]),
  });

  const stop = pq.inspect();
  const bestPaths = new Set();
  const visited = new Map(); // Tracks the lowest score to each (x, y, dirIndex)

  while (!pq.isEmpty()) {
    await new Promise((r) => setTimeout(r, 0));
    const item = pq.dequeue();
    const { x, y, dirIndex, score, path } = item;
    const key = `${x},${y},${dirIndex}`;

    const currScore = visited.get(key);
    if (currScore !== undefined && currScore < score) continue;
    if (currScore !== undefined && currScore > optimalScore) continue;
    visited.set(key, score);
    if (x === end.x && y === end.y) {
      if (score === optimalScore) {
        path.forEach((tile) => bestPaths.add(tile));
      }
      continue;
    }
    // Move forward
    const nx = x + directions[dirIndex].dx;
    const ny = y + directions[dirIndex].dy;
    if (isValidMove(nx, ny, map)) {
      const newPath = new Set(path);
      newPath.add(`${nx},${ny}`);
      pq.enqueue({
        x: nx,
        y: ny,
        dirIndex,
        score: score + 1,
        path: newPath,
      });
    }

    // Rotate clockwise or counterclockwise
    for (let turn = -1; turn <= 1; turn += 2) {
      const newDirIndex = (dirIndex + turn + 4) % 4;

      pq.enqueue({
        x,
        y,
        dirIndex: newDirIndex,
        score: score + 1000,
        path: path,
      });
    }
  }
  stop();
  markBestPaths(map, bestPaths);

  return bestPaths.size;
}
async function main() {
  const input = parse(await readFromStdin());
  const p1 = partOne(input);
  console.log(p1);
  console.log(await partTwo(input, p1));
}
main();
