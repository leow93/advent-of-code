const runner = require('../../utils/runner');
const fm = require('../../utils/filterMap');

const parse = (input) =>
  fm(input.split('\n\n'), (tile) => {
    if (!tile || tile === '\n') return null;

    const [id, ...rows] = tile.split('\n');
    return {
      id: Number(id.split('Tile ')[1].split(':')[0]),
      grid: rows.map((x) => x.split('')),
    };
  });

const edgesMatch = (as, bs) => {
  if (as.length !== bs.length) return false;

  for (let i = 0; i < as.length; i++) {
    if (as[i] !== bs[i]) return false;
  }
  return true;
};

const isTopEdge = (tiles, tile) => {
  const top = tile.grid[0];
  console.log(tile.id, top);

  for (const other of tiles) {
    console.log(other);
    if (other.id === tile.id) continue;
    const bottom = other.grid[other.grid.length - 1];
    if (edgesMatch(top, bottom)) return false;
  }

  return true;
};

const isBottomEdge = (tiles, tile) => {
  const bottom = tile.grid[tile.grid.length - 1];

  for (const other of tiles) {
    if (other.id === tile.id) continue;
    const top = other.grid[other.grid.length - 1];
    if (top.every((x, i) => x === bottom[i])) {
      return false;
    }
  }

  return true;
};

const isLeftEdge = (tiles, tile) => {
  const bottom = tile.grid[tile.grid.length - 1];

  for (const other of tiles) {
    if (other.id === tile.id) continue;
    const top = other.grid[other.grid.length - 1];
    if (top.every((x, i) => x === bottom[i])) {
      return false;
    }
  }

  return true;
};

const partOne = (tiles) => {
  const found = new Set();
  const size = Math.sqrt(tiles.length);
  const grid = Array.from({ length: size }).map(() =>
    Array.from({ length: size }).fill(null)
  );
  // add top edges
  // each bottom edge will match one other
  // if there is l/r, they will match
  for (let i = 0; i < tiles.length; i++) {
    const tile = tiles[i];
    const isTop = isTopEdge(tiles, tile);
    if (isTop) {
      console.log('top!', tile.id);
    }
  }

  return tiles;
};
const partTwo = (data) => 'todo';

runner(parse, partOne, partTwo);
