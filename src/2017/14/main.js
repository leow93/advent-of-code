const runner = require('../../utils/runner');

const parse = (input) => input.split('\n')[0];

// #begin copy from part 10
const index = (xs, i) => {
  if (i < 0) {
    return xs.length + (i % xs.length);
  }

  return (i + xs.length) % xs.length;
};

const get = (xs, i) => {
  return xs[index(xs, i)];
};
const select = (xs, start, len) => {
  let i = start;
  const result = [];
  while (result.length !== len) {
    result.push(get(xs, i));
    i++;
  }

  return result;
};

const reverseSection = (xs, start, len) => {
  const result = xs.slice();
  const reversed = select(xs, start, len).reverse();

  for (let i = 0; i < reversed.length; i++) {
    result[index(result, start + i)] = reversed[i];
  }

  return result;
};

const knotHash = (input, curr = 0, skip = 0, rounds = 1) => {
  const xs = Array.from({ length: 256 }).map((_, i) => i);
  const seq = input.slice();

  // i used as skip as well as index
  const loop = (xs, curr, skip, i, round) => {
    if (i >= seq.length) {
      return round === rounds ? xs : loop(xs, curr, skip, 0, round + 1);
    }
    const length = seq[i];
    const nextXs = reverseSection(xs, curr, length);
    return loop(nextXs, curr + length + skip, skip + 1, i + 1, round);
  };
  return loop(xs, curr, skip, 0, 1);
};

const xor = (...xs) => {
  let result = xs[0];
  for (let i = 1; i < xs.length; i++) {
    result = result ^ xs[i];
  }
  return result;
};

const denseHash = (sparse) => {
  if (sparse.length !== 256)
    throw new Error('should have 256 elements, got ' + sparse.length);

  const result = [];
  for (let i = 0; i < 16; i++) {
    const elems = sparse.slice(i * 16, i * 16 + 16);
    result.push(xor(...elems));
  }

  return result.map((x) => x.toString(16).padStart(2, '0')).join('');
};

const hash = (data) => {
  const ascii = data
    .split('')
    .map((x) => x.charCodeAt(0))
    .join(',');
  const input = (ascii + ',17,31,73,47,23').split(',').map(Number);
  const sparseHash = knotHash(input, 0, 0, 64);
  return denseHash(sparseHash);
};

// #end copy from part 10

const hexToBinary = (x) =>
  parseInt(x.toString(), 16).toString('2').padStart(4, '0');
const hashToBinary = (xs) => {
  return xs.split('').map(hexToBinary);
};

const makeGrid = (input) => {
  const inputs = Array.from({ length: 128 }).map((_, i) => `${input}-${i}`);
  const grid = [];
  for (const x of inputs) {
    const h = hash(x);
    const binary = hashToBinary(h);
    const row = binary.flatMap((x) => x.split(''));
    grid.push(row);
  }

  return grid;
};

const partOne = (input) => {
  // 8176 too low
  return makeGrid(input).reduce(
    (count, row) => count + row.filter((x) => x === '1').length,
    0
  );
};

const neighbours = (grid, i, j) =>
  [
    [i - 1, j],
    [i + 1, j],
    [i, j - 1],
    [i, j + 1],
  ].filter(
    ([x, y]) => x >= 0 && x < grid.length && y >= 0 && y < grid[0].length
  );

const getRegions = (input) => {
  const grid = makeGrid(input);
  const visited = new Set();

  const id = (y, x) => `${y},${x}`;

  const regions = [];

  const explore = (y, x) => {
    const result = [[y, x]];
    const queue = neighbours(grid, y, x);
    while (queue.length) {
      const [y, x] = queue.pop();
      if (visited.has(id(y, x))) continue;
      visited.add(id(y, x));
      if (grid[y][x] === '0') continue;

      result.push([y, x]);
      for (const n of neighbours(grid, y, x)) {
        queue.push(n);
      }
    }

    return result;
  };

  for (let y = 0; y < grid.length; y++) {
    for (let x = 0; x < grid[y].length; x++) {
      const value = grid[y][x];
      const key = id(y, x);
      if (visited.has(key)) continue;
      visited.add(key);
      if (value === '0') {
        continue;
      }

      const region = [[y, x]];
      const queue = neighbours(grid, y, x);
      while (queue.length) {
        const [i, j] = queue.pop();
        if (visited.has(id(i, j))) continue;
        visited.add(id(i, j));
        if (grid[i][j] === '0') continue;

        region.push([i, j]);
        for (const n of neighbours(grid, i, j)) {
          queue.push(n);
        }
      }

      if (region.length > 0) {
        regions.push(region);
      }
    }
  }

  return regions;
};

const getRegionsOld = (input) => {
  const grid = makeGrid(input);
  const visited = new Set();
  const stack = grid
    .flatMap((row, i) => row.map((value, j) => ({ y: i, x: j, value })))
    .reverse();
  let count = 0;

  const regions = [{ coords: new Set() }];
  while (stack.length > 0) {
    const { y: i, x: j, value } = stack.pop();
    const key = `${i},${j}`;
    if (visited.has(key)) continue;
    visited.add(key);

    const region = regions.pop();

    if (value === '0') {
      regions.push(region);
      regions.push({ coords: new Set() });
      count++;
      continue;
    }
    region.coords.add(key);

    for (const [y, x] of neighbours(grid, i, j)) {
      if (grid[y][x] === '1') {
        stack.push({ value: '1', x, y });
      }
    }

    regions.push(region);
  }

  console.log({ count });

  return regions.filter((r) => r.coords.size > 0);
};

const partTwo = (input) => {
  return getRegions(input).length;
};

runner(parse, partOne, partTwo);
