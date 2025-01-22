const runner = require('../../utils/runner');

const parse = (input) => input.split(',').map((x) => x.replace(/\s/, ''));

// Using cube co-ordinates
const directions = {
  n: [0, -1, 1],
  ne: [1, -1, 0],
  se: [1, 0, -1],
  s: [0, 1, -1],
  sw: [-1, 1, 0],
  nw: [-1, 0, 1],
};

const reducer = (vector, d) => {
  const direction = directions[d];
  if (!direction) return vector;
  return vector.map((x, i) => x + direction[i]);
};

const sum = (ds) => ds.reduce(reducer, [0, 0, 0]);

const distance = (coords) => Math.max(...coords.map(Math.abs));

const partOne = (data) => {
  return distance(sum(data));
};
const partTwo = (data) => {
  let state = [0, 0, 0];
  let max = 0;
  for (const d of data) {
    state = reducer(state, d);
    const dist = distance(state);
    if (dist > max) max = dist;
  }

  return max;
};

runner(parse, partOne, partTwo);
