const fs = require('fs');
const path = require('path');

const readLines = (name) =>
  fs.readFileSync(path.join(__dirname, name)).toString('utf-8').split('\n');

const range = (a, b) => Array.from({ length: b - a + 1 }).map((_, i) => a + i);

const toRange = (s) => {
  const [from, to] = s.split('-');
  if (!from || !to) return undefined;
  return range(Number(from), Number(to));
};

const isSuperSet = (a, b) => {
  const diff = new Set();
  for (const x of b.values()) {
    if (!a.has(x)) {
      diff.add(x);
    }
  }
  return diff.size === 0;
};

const intersection = (a, b) => {
  const result = new Set();
  for (const x of a.values()) {
    if (b.has(x)) {
      result.add(x);
    }
  }
  return result;
};

Array.prototype.sumBy = function (fn) {
  let sum = 0;
  for (const x of this.values()) {
    sum += fn(x);
  }
  return sum;
};

const parseData = (file) =>
  readLines(file)
    .map((line) => {
      const [a, b] = line.split(',');
      if (!a || !b) return undefined;

      const rangeA = toRange(a);
      const rangeB = toRange(b);
      if (!rangeA || !rangeB) return undefined;
      return [rangeA, rangeB];
    })
    .filter(Boolean);

const partOne = (file) =>
  parseData(file).sumBy(([a, b]) => {
    const setA = new Set([...a]);
    const setB = new Set([...b]);
    return isSuperSet(setA, setB) || isSuperSet(setB, setA) ? 1 : 0;
  });

const partTwo = (file) =>
  parseData(file).sumBy(([a, b]) => {
    const setA = new Set([...a]);
    const setB = new Set([...b]);
    return intersection(setA, setB).size > 0 ? 1 : 0;
  });

console.log('Test');
console.log('Part I: ', partOne('test.txt'));
console.log('Part II: ', partTwo('test.txt'));

console.log('\nActual');
console.log('Part I: ', partOne('data.txt'));
console.log('Part II: ', partTwo('data.txt'));
