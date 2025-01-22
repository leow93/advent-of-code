const runner = require('../../utils/runner');

const parse = (input) => input.split('\n')[0];
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

const partOne = (data) => {
  const input = data.split(',').map(Number);
  const arr = knotHash(input);
  return arr[0] * arr[1];
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

  return result.map((x) => x.toString(16)).join('');
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

const partTwo = hash;

runner(parse, partOne, partTwo);
