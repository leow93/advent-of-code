const filterMap = require('../../utils/filterMap');
const runner = require('../../utils/runner');

const parse = (input) =>
  filterMap(input.split('\n'), (line) =>
    !line || line === '\n' ? null : line.split('')
  );

const tryCell = (grid, [i, j]) => grid[i]?.[j] ?? null;
const deltas = [
  [-1, 0],
  [1, 0],
  [0, 1],
  [0, -1],
];

const addCoord = (a, b) => [a[0] + b[0], a[1] + b[1]];

function neighbours(grid, coord) {
  const result = [];
  for (const d of deltas) {
    const c = addCoord(coord, d);
    const n = tryCell(grid, c);
    if (n != null) {
      result.push(c);
    }
  }
  return result;
}

const getRegions = (grid) => {
  const rows = grid.length;
  const cols = grid[0].length;

  const visited = Array.from({ length: rows }, () => Array(cols).fill(false));
  const isValid = (x, y, char) => {
    return (
      x >= 0 &&
      x < rows &&
      y >= 0 &&
      y < cols &&
      !visited[x][y] &&
      grid[x][y] === char
    );
  };

  const dfs = (x, y, char, region) => {
    visited[x][y] = true;
    region.push([x, y]);
    for (const [i, j] of neighbours(grid, [x, y])) {
      if (isValid(i, j, char)) {
        dfs(i, j, char, region);
      }
    }
  };

  const regions = [];

  for (let i = 0; i < rows; i++) {
    for (let j = 0; j < cols; j++) {
      if (!visited[i][j]) {
        const region = [];
        dfs(i, j, grid[i][j], region);
        regions.push(region);
      }
    }
  }

  return regions;
};

const countRegionalNeighbours = (grid, i, j, char) => {
  return neighbours(grid, [i, j]).filter(([x, y]) => grid[x][y] === char)
    .length;
};

const getPrice = (grid, region) => {
  const area = region.length;
  let perimeter = 0;

  for (const [x, y] of region) {
    perimeter += 4 - countRegionalNeighbours(grid, x, y, grid[x][y]);
  }

  return area * perimeter;
};

const countCorners = (region) => {
  let corners = 0;

  const isInRegion = ([x, y]) => {
    for (const [i, j] of region) {
      if (i === x && j === y) {
        return true;
      }
    }
    return false;
  };

  for (const [x, y] of region) {
    for (const [dx, dy] of [
      [1, 1],
      [1, -1],
      [-1, 1],
      [-1, -1],
    ]) {
      const diagonalRow = x + dx;
      const diagonalCol = y + dy;
      const neighbourOne = [x, diagonalCol];
      const neighbourTwo = [diagonalRow, y];

      if (isInRegion([diagonalRow, diagonalCol])) {
        // The diagonal is part of the region. This can only be a corner piece if
        // the neighbouring squares aren't part of the shape.
        // e.g. considering the central A, bottom right is a corner because B is not in the shape
        // AAA
        // AAB
        // AAA
        if (!isInRegion(neighbourOne) && !isInRegion(neighbourTwo)) {
          corners++;
        }
      } else if (isInRegion(neighbourOne) === isInRegion(neighbourTwo)) {
        // diagonal is not part of the shape,
        // but either both neighbours are or neither of them are, so it's a corner
        // e.g. considering the central A again, bottom right is a corner in both these examples:
        //
        // AAA
        // AAA
        // AAB
        //
        // or
        //
        // AAA
        // AAB
        // ABB
      }
      corners++;
    }
  }

  return corners;
};

const getDiscountedPrice = (_grid, region) => {
  const area = region.length;
  const sides = countCorners(region);
  return area * sides;
};

const sum = (grid, pricing) => (result, region) =>
  result + pricing(grid, region);

const partOne = (grid) => {
  const rs = getRegions(grid);
  return rs.reduce(sum(grid, getPrice), 0);
};
const partTwo = (grid) => {
  const rs = getRegions(grid);
  return rs.reduce(sum(grid, getDiscountedPrice), 0);
};

if (require.main === module) runner(parse, partOne, partTwo);
