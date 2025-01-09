const runner = require('../../utils/runner');

const toKey = (xs) => xs.join(',');
const fromKey = (key) => key.split(',').map(Number);

const parse = (input) => {
  const grid3 = new Map();
  const grid4 = new Map();
  const rows = input.split('\n');
  for (let y = 0; y < rows.length; y++) {
    const line = rows[y];
    if (!line || line === '\n') continue;
    const cols = line.split('');
    for (let x = 0; x < cols.length; x++) {
      const key3 = toKey([x, y, 0]);
      const key4 = toKey([x, y, 0, 0]);
      const value = cols[x] === '#';
      grid3.set(key3, value);
      grid4.set(key4, value);
    }
  }

  return [grid3, grid4];
};

const neigboursGen = (dimensions) => {
  // Generate all possible combinations of [-1, 0, 1] for the given dimensions
  function generateCombos(current, depth) {
    if (depth === dimensions) {
      combos.push([...current]);
      return;
    }
    for (let d of [-1, 0, 1]) {
      current.push(d);
      generateCombos(current, depth + 1);
      current.pop();
    }
  }

  const combos = [];
  generateCombos([], 0);

  // Filter out the origin (all zeros)
  return combos.filter((combo) => !combo.every((val) => val === 0));
};
const neighbours3 = neigboursGen(3);
const neighbours4 = neigboursGen(4);

const boundariesGen = (n) => (grid) => {
  const result = Array.from({ length: n }).map(() => [Infinity, -Infinity]);

  for (key of grid.keys()) {
    const xs = fromKey(key);
    for (let i = 0; i < xs.length; i++) {
      const x = xs[i];
      const min = result[i][0];
      const max = result[i][1];
      if (x < min) result[i][0] = x;
      if (x > max) result[i][1] = x;
    }
  }

  return result.map(([min, max]) => [min - 1, max + 1]);
};
const boundaries3 = boundariesGen(3);
const boundaries4 = boundariesGen(4);

const runCycle3 = (grid) => {
  const result = new Map(grid);

  const [[minX, maxX], [minY, maxY], [minZ, maxZ]] = boundaries3(grid);

  for (let x = minX; x <= maxX; x++) {
    for (let y = minY; y <= maxY; y++) {
      for (let z = minZ; z <= maxZ; z++) {
        let activeNeighbours = 0;
        const key = toKey([x, y, z]);
        const active = grid.get(key) === true;
        for (const [dx, dy, dz] of neighbours3) {
          const key = toKey([x + dx, y + dy, z + dz]);
          if (grid.get(key)) activeNeighbours++;
        }

        // If a cube is active and exactly 2 or 3 of its neighbors are also active, the cube remains active. Otherwise, the cube becomes inactive.
        if (active) {
          const newActive = activeNeighbours === 2 || activeNeighbours === 3;
          result.set(key, newActive);
          continue;
        }

        if (!active && activeNeighbours === 3) {
          result.set(key, true);
          continue;
        }
      }
    }
  }

  return result;
};

const runCycle4 = (grid) => {
  const result = new Map(grid);

  const [[minX, maxX], [minY, maxY], [minZ, maxZ], [minW, maxW]] =
    boundaries4(grid);

  for (let x = minX; x <= maxX; x++) {
    for (let y = minY; y <= maxY; y++) {
      for (let z = minZ; z <= maxZ; z++) {
        for (let w = minW; w <= maxW; w++) {
          let activeNeighbours = 0;
          const key = toKey([x, y, z, w]);
          const active = grid.get(key) === true;
          for (const [dx, dy, dz, dw] of neighbours4) {
            const key = toKey([x + dx, y + dy, z + dz, w + dw]);
            if (grid.get(key)) activeNeighbours++;
          }

          // If a cube is active and exactly 2 or 3 of its neighbors are also active, the cube remains active. Otherwise, the cube becomes inactive.
          if (active) {
            const newActive = activeNeighbours === 2 || activeNeighbours === 3;
            result.set(key, newActive);
            continue;
          }

          if (!active && activeNeighbours === 3) {
            result.set(key, true);
            continue;
          }
        }
      }
    }
  }

  return result;
};

const run = (data, runner) => {
  let grid = data;
  let n = 0;
  while (n < 6) {
    grid = runner(grid);
    n++;
  }

  let count = 0;
  for (const active of grid.values()) {
    if (active) count++;
  }

  return count;
};

const partOne = ([data]) => run(data, runCycle3);

const partTwo = ([_, data]) => run(data, runCycle4);

runner(parse, partOne, partTwo);
