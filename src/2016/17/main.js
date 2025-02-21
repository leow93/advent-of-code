const crypto = require('crypto');
const runner = require('../../utils/runner');

const md5 = (string) => {
  const hash = crypto.createHash('md5');
  hash.update(string);
  return hash.digest('hex').toLowerCase().slice(0, 4);
};

const grid = [
  ['S', '', '', ''],
  ['', '', '', ''],
  ['', '', '', ''],
  ['', '', '', 'V'],
];

const parse = (data) => data;

const openDoors = ['b', 'c', 'd', 'e', 'f'];
const open = (x) => openDoors.includes(x);

const directions = [
  { id: 'U', delta: [-1, 0] },
  { id: 'D', delta: [1, 0] },
  { id: 'L', delta: [0, -1] },
  { id: 'R', delta: [0, 1] },
];

const bfs = (password) => {
  const queue = [{ x: 0, y: 0, path: '' }];
  const paths = [];
  while (queue.length) {
    const { x, y, path } = queue.shift();
    if (grid[y][x] === 'V') {
      paths.push(path);
      continue;
    }

    const hash = md5(password + path);
    for (let i = 0; i < directions.length; i++) {
      const {
        id,
        delta: [dy, dx],
      } = directions[i];
      const ch = hash[i];
      const y1 = y + dy;
      const x1 = x + dx;
      const exists =
        y1 >= 0 && y1 < grid.length && x1 >= 0 && x1 < grid[y1].length;

      if (exists && open(ch)) {
        const newPath = path + id;
        queue.push({ path: newPath, x: x1, y: y1 });
      }
    }
  }

  return paths;
};

const reduce = (f, s) => (paths) => paths.reduce(f, s);
const run = (f, s) => (pwd) => reduce(f, s)(bfs(pwd));

const partOne = run((acc, path) => (path.length < acc.length ? path : acc), {
  length: Infinity,
});
const partTwo = run((max, path) => (path.length > max ? path.length : max), 0);

runner(parse, partOne, partTwo);
