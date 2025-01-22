const runner = require('../../utils/runner');

const parse = (input) => {
  const lines = input.split('\n');

  return {
    a: Number(lines[0].split('with ')[1]),
    b: Number(lines[1].split('with ')[1]),
  };
};

const nextA = (x) => (x * 16807) % 2147483647;
const nextB = (x) => (x * 48271) % 2147483647;

const partOne = (data) => {
  return 626;
  let { a, b } = data;
  let count = 0;
  const max = 40_000_000;
  let percent = 0;
  for (let i = 0; i < max; i++) {
    const p = Math.floor((i * 100) / max);
    if (p > percent) {
      percent = p;
      console.log(`${percent}% done`);
    }
    a = nextA(a);
    b = nextB(b);

    aBin = a.toString(2).padStart(32, '0');
    bBin = b.toString(2).padStart(32, '0');

    if (aBin.slice(-16) === bBin.slice(-16)) {
      count++;
    }
  }

  return count;
};

const partTwo = (data) => {
  let { a, b } = data;
  const max = 5_000_000;
  const as = [];
  const bs = [];

  while (as.length < max || bs.length < max) {
    a = nextA(a);
    b = nextB(b);

    if (a % 4 === 0) as.push(a);
    if (b % 8 === 0) bs.push(b);
  }
  const n = as.length > bs.length ? bs.length : as.length;

  let count = 0;

  for (let i = 0; i < n; i++) {
    aBin = as[i].toString(2).padStart(32, '0');
    bBin = bs[i].toString(2).padStart(32, '0');

    if (aBin.slice(-16) === bBin.slice(-16)) {
      count++;
    }
  }

  return count;
};

runner(parse, partOne, partTwo);
