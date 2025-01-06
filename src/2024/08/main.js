const runner = require('../../utils/runner');

const parse = (input) => {
  const lines = input.split('\n');
  const map = new Map();
  const grid = [];
  for (let i = 0; i < lines.length; i++) {
    const line = lines[i];
    if (!line) continue;
    const row = [];
    for (let j = 0; j < line.length; j++) {
      const cell = line[j];
      row.push(cell);
      if (cell !== '.') {
        map.set(
          cell,
          map.get(cell) ? map.get(cell).concat([[i, j]]) : [[i, j]]
        );
      }
    }

    grid.push(row);
  }

  return {
    grid,
    map,
  };
};

const getPairs = (coords) => {
  const loop = (idx, result) => {
    if (idx >= coords.length) {
      return result;
    }
    const coord = coords[idx];
    const tmp = [];
    for (let i = idx + 1; i < coords.length; i++) {
      tmp.push([coord, coords[i]]);
    }

    return loop(idx + 1, result.concat(tmp));
  };

  return loop(0, []);
};

const valid = (grid, [i, j]) =>
  i >= 0 && i < grid.length && j >= 0 && j < grid[0].length;

const getAdjacentAntinodes = (grid, map, [i, j]) => {
  const cell = grid[i][j];
  if (cell === '.') {
    return [];
  }

  const nodes = map.get(cell);
  if (!nodes) return [];
  const pairs = getPairs(nodes);

  const antinodes = [];
  for (const [a, b] of pairs) {
    const dy = b[0] - a[0];
    const dx = b[1] - a[1];
    const n1 = [a[0] - dy, a[1] - dx];
    const n2 = [b[0] + dy, b[1] + dx];
    if (valid(grid, n1)) {
      antinodes.push(n1);
    }
    if (valid(grid, n2)) {
      antinodes.push(n2);
    }
  }

  return antinodes;
};

const line = (grid, start, dx, dy) => {
  const result = [start];
  let count = 1;
  while (true) {
    const next = [start[0] + dy * count, start[1] + dx * count];
    const ok = valid(grid, next);
    if (!ok) {
      break;
    }
    result.push(next);
    count++;
  }
  return result;
};

const getAllAntiNodes = (grid, map, [i, j]) => {
  const cell = grid[i][j];
  if (cell === '.') {
    return [];
  }

  const nodes = map.get(cell);
  if (!nodes) return [];
  const pairs = getPairs(nodes);
  const antinodes = [];
  for (const [a, b] of pairs) {
    const dy = b[0] - a[0];
    const dx = b[1] - a[1];

    const as = line(grid, [a[0], a[1]], dx, dy);
    const bs = line(grid, [b[0], b[1]], -dx, -dy);
    antinodes.push(...as, ...bs);
  }

  return antinodes;
};

const findAntinodes =
  (project) =>
  ({ grid, map }) => {
    const visited = new Set();
    const antinodes = [];
    for (let i = 0; i < grid.length; i++) {
      const row = grid[i];
      for (let j = 0; j < row.length; j++) {
        const cell = row[j];
        if (cell === '.' || visited.has(cell)) {
          continue;
        }
        visited.add(cell);
        antinodes.push(...project(grid, map, [i, j]));
      }
    }
    return antinodes;
  };

const countUniqueCoords = (coords) => {
  const set = new Set();
  for (const [x, y] of coords) {
    set.add(x.toString() + ':' + y.toString());
  }
  return set.size;
};

const run = (project) => (data) => {
  const find = findAntinodes(project);
  const antinodes = find(data);
  return countUniqueCoords(antinodes);
};

const partOne = run(getAdjacentAntinodes);
const partTwo = run(getAllAntiNodes);

if (require.main === module) {
  runner(parse, partOne, partTwo);
}
