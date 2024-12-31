const get = (grid, coord) => grid[coord.y]?.[coord.x];
const left = (coord) => ({ x: coord.x - 1, y: coord.y });
const right = (coord) => ({ x: coord.x + 1, y: coord.y });
const up = (coord) => ({ x: coord.x, y: coord.y - 1 });
const down = (coord) => ({ x: coord.x, y: coord.y + 1 });

const movers = {
  '<': left,
  '>': right,
  v: down,
  '^': up,
};

module.exports = {
  movers,
  get,
  left,
  right,
  up,
  down,
};
