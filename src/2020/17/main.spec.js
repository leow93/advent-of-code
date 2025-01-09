const test = (desc, f) => {
  try {
    f();
    console.log(desc + ': OK');
  } catch (e) {
    console.error(desc + ': FAILED');
    console.error(e);
  }
};

const describe = (desc, f) => {
  console.log(desc);
  f();
  console.log('======================');
};

const combinations = (xs) => {
  return xs;
};

describe('empty array', () => {
  test('begets empty', () => {
    const result = combinations([]);
    if (result.length !== 0) {
      throw new Error('expecte empty array');
    }
  });
});

class UnequalError extends Error {
  constructor(a, b) {
    super('Unequal');
    console.log({
      a,
      b,
    });
  }
}

const arrEq = (xs, ys) => {
  if (xs.length !== ys.length) return false;

  for (let i = 0; i < xs.length; i++) {
    if (xs[i] !== ys[i]) return false;
  }
  return true;
};

describe('array of 1 element', () => {
  test('returns array', () => {
    const result = combinations([1]);
    if (!arrEq([1], result)) throw new UnequalError([1], result);
  });
});

describe('array of 2 elements', () => {});

const neighbours = [
  [0, 0, 1],
  [0, 0, -1],
  [0, 1, 0],
  [0, 1, 1],
  [0, 1, -1],
  [0, -1, 1],
  [0, -1, -1],

  [1, 0, 1],
  [1, 0, -1],
  [1, 1, 0],
  [1, 1, 1],
  [1, 1, -1],
  [1, -1, 1],
  [1, -1, -1],

  [-1, 0, 1],
  [-1, 0, -1],
  [-1, 1, 0],
  [-1, 1, 1],
  [-1, 1, -1],
  [-1, -1, 1],
  [-1, -1, -1],
];
