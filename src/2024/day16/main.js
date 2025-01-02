const filterMap = require('../../utils/filterMap');
const runner = require('../../utils/runner');

const parse = (input) => {
  return input.split('\n').map((row) => row.split(''));
};

class MinPriorityQueue {
  constructor({ priority }) {
    this.heap = [];
    this.priority = priority;
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

function partOne(map) {
  const numRows = map.length;
  const numCols = map[0].length;

  const directions = [
    { dx: 1, dy: 0 }, // East
    { dx: 0, dy: 1 }, // South
    { dx: -1, dy: 0 }, // West
    { dx: 0, dy: -1 }, // North
  ];

  const start = findStart(map);
  const end = findEnd(map);

  // Priority queue for Dijkstra's algorithm
  const pq = new MinPriorityQueue({ priority: (a, b) => a.score - b.score });
  pq.enqueue({ x: start.x, y: start.y, dirIndex: 0, score: 0 });

  const visited = new Set();

  while (!pq.isEmpty()) {
    const { x, y, dirIndex, score } = pq.dequeue();
    const key = `${x},${y},${dirIndex}`;

    if (visited.has(key)) continue;
    visited.add(key);

    // Check if we've reached the end
    if (x === end.x && y === end.y) {
      return score;
    }

    // Move forward
    const nx = x + directions[dirIndex].dx;
    const ny = y + directions[dirIndex].dy;
    if (isValidMove(nx, ny, map)) {
      pq.enqueue({ x: nx, y: ny, dirIndex, score: score + 1 });
    }

    // Rotate clockwise or counterclockwise
    for (let turn = -1; turn <= 1; turn += 2) {
      const newDirIndex = (dirIndex + turn + 4) % 4;
      pq.enqueue({ x, y, dirIndex: newDirIndex, score: score + 1000 });
    }
  }

  return -1; // If no path found

  function isValidMove(x, y, map) {
    return x >= 0 && x < numCols && y >= 0 && y < numRows && map[y][x] !== '#';
  }

  function findStart(map) {
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
    for (let y = 0; y < numRows; y++) {
      for (let x = 0; x < numCols; x++) {
        if (map[y][x] === 'E') {
          return { x, y };
        }
      }
    }
    throw new Error('End tile not found');
  }
}

const partTwo = (data) => 'todo';

runner(parse, partOne, partTwo);
