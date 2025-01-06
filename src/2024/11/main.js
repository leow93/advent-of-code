const runner = require('../../utils/runner');

const inc = (map, key, amount) => {
  const curr = map[key] ?? 0;
  map[key] = curr + amount;
};

const dec = (map, key, amount) => {
  const curr = map[key] ?? 0;
  const next = curr - amount;
  if (next <= 0) {
    delete map[key];
    return;
  }
  map[key] = next;
};

const sum = (map) => {
  let total = 0;
  for (const v of Object.values(map)) {
    total += v;
  }
  return total;
};

const parse = (input) => {
  const entries = input.split(' ').map((x) => x.replaceAll('\n', ''));
  const map = {};

  for (const e of entries) {
    inc(map, e, 1);
  }
  return map;
};

const reduce = (map, { stone, count }) => {
  if (stone === '0') {
    inc(map, '1', count);
    dec(map, '0', count);
    return;
  }

  if (stone.length % 2 === 0) {
    const first = stone.slice(0, stone.length / 2);
    const second = stone.slice(stone.length / 2);

    inc(map, first, count);
    inc(map, Number(second).toString(), count);
    dec(map, stone, count);
    return;
  }

  dec(map, stone, count);
  inc(map, (Number(stone) * 2024).toString(), count);
};

const blink = (map) => {
  for (const [stone, count] of Object.entries(map)) {
    reduce(map, { stone, count });
  }
};

const run = (n) => (map) => {
  let i = 0;
  let result = { ...map };
  while (i < n) {
    blink(result);
    i++;
  }
  return sum(result);
};

const partOne = run(25);

const partTwo = run(75);

runner(parse, partOne, partTwo);
