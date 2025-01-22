const makeGrid = (input) => input.split('\n').map((row) => row.split(''));

const neighbours = (grid, i, j) =>
  [
    [i - 1, j],
    [i + 1, j],
    [i, j - 1],
    [i, j + 1],
  ].filter(
    ([y, x]) => y >= 0 && y < grid.length && x >= 0 && x < grid[0].length
  );

const getRegionsOld = (input) => {
  const grid = makeGrid(input);
  const visited = new Set();
  const stack = grid
    .flatMap((row, i) => row.map((value, j) => ({ y: i, x: j, value })))
    .reverse();

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

  return regions.filter((r) => r.coords.size > 0);
};

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

      // part of region
      regions.push(explore(y, x));
    }
  }

  return regions;
};

const containsCoord = (xs, coord) =>
  xs.some(([a, b]) => a === coord[0] && b === coord[1]);

test('gets regions correctly', () => {
  const input = `111
010
000`;
  const regions = getRegions(input);

  expect(regions).toHaveLength(1);
  expect(regions[0].length).toBe(4);
  expect(containsCoord(regions[0], [0, 0])).toBe(true);
  expect(containsCoord(regions[0], [0, 1])).toBe(true);
  expect(containsCoord(regions[0], [0, 2])).toBe(true);
  expect(containsCoord(regions[0], [1, 1])).toBe(true);
});

test('gets regions correctly again', () => {
  const input = `01100
00000
10010
11110
00010`;
  const regions = getRegions(input);

  expect(regions).toHaveLength(2);
  expect(regions[0].length).toBe(2);
  expect(containsCoord(regions[0], [0, 1])).toBe(true);
  expect(containsCoord(regions[0], [0, 2])).toBe(true);

  expect(regions[1].length).toBe(7);
  expect(containsCoord(regions[1], [2, 0])).toBe(true);
  expect(containsCoord(regions[1], [2, 3])).toBe(true);
  expect(containsCoord(regions[1], [3, 0])).toBe(true);
  expect(containsCoord(regions[1], [3, 1])).toBe(true);
  expect(containsCoord(regions[1], [3, 2])).toBe(true);
  expect(containsCoord(regions[1], [3, 3])).toBe(true);
  expect(containsCoord(regions[1], [4, 3])).toBe(true);
});

test('gets regions correctly again', () => {
  const input = `01100000000
00110000000
00011111000
00000001000
00000001000
00000001000
00000001000
00000001000
00000001000
00000001000
00000001000`;
  const regions = getRegions(input);

  expect(regions).toHaveLength(1);
  expect(regions[0].length).toBe(17);
  expect(regions[0]).toEqual([
    [0, 1],
    [0, 2],
    [1, 2],
    [1, 3],
    [2, 3],
    [2, 4],
    [2, 5],
    [2, 6],
    [2, 7],
    [3, 7],
    [4, 7],
    [5, 7],
    [6, 7],
    [7, 7],
    [8, 7],
    [9, 7],
    [10, 7],
  ]);
});

test('gets regions correctly again (again)', () => {
  const input = `01100000000
00110000000
00011111000
01000001000
01111101000
00011101000
00011101000
00000001000
00000001000
00000001000
00000001000`;
  const regions = getRegions(input);

  expect(regions).toHaveLength(2);
  expect(regions[0].length).toBe(17);
  expect(regions[0]).toEqual([
    [0, 1],
    [0, 2],
    [1, 2],
    [1, 3],
    [2, 3],
    [2, 4],
    [2, 5],
    [2, 6],
    [2, 7],
    [3, 7],
    [4, 7],
    [5, 7],
    [6, 7],
    [7, 7],
    [8, 7],
    [9, 7],
    [10, 7],
  ]);

  expect(regions[1].length).toBe(12);
});
