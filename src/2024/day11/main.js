const runner = require('../../utils/runner');

const parse = (input) => {
  const entries = input.split(' ').map((x) => x.replaceAll('\n', ''));
  const map = new CountMap();
  for (const e of entries) {
    map.inc(e, 1);
  }
  return map;
};

class CountMap {
  constructor(m) {
    this.data = new Map(m);
  }

  inc(k, n) {
    const curr = this.data.get(k) ?? 0;
    this.data.set(k, curr + n);
  }

  dec(k, n) {
    const curr = this.data.get(k) ?? 0;
    if (curr === 0) return;
    const next = curr - n;
    if (next <= 0) {
      this.data.delete(k);
      return;
    }
    this.data.set(k, next);
  }

  total() {
    let count = 0;
    for (const [_, x] of this.data.entries()) {
      count += x;
    }
    return count;
  }

  *[Symbol.iterator]() {
    for (const [key, value] of this.data.entries()) {
      yield [key, value];
    }
  }
}

const reduce = (map, { stone, count }) => {
  const result = new CountMap(map);
  if (stone === '0') {
    result.inc('1', count);
    result.dec('0', count);
    return result;
  }

  if (stone.length % 2 === 0) {
    const first = stone.slice(0, stone.length / 2);
    const second = stone.slice(stone.length / 2);
    result.dec(stone, count);
    result.inc(first, count);
    result.inc(Number(second).toString(), count);
    return result;
  }

  result.dec(stone, count);
  result.inc((Number(stone) * 2024).toString(), count);
  return result;
};

const blink = (map) => {
  for (const [stone, count] of map) {
    map = reduce(map, { stone, count });
  }
  return map;
};

const blinkN = (map, n) => {
  let i = 0;
  while (i < n) {
    map = blink(map);
    i++;
  }
  return map;
};

const run = (n) => (stones) => blinkN(stones, n).total();

const partOne = run(25);
const partTwo = run(75);

runner(parse, partOne, partTwo);
