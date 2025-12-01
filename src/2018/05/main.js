const runner = require('../../utils/runner');

const parse = (input) => input.split('').filter((x) => x && x !== '\n');

const toTuple = (arr) => {
  if (arr.length < 2) return [];
  const result = [];

  for (let i = 0; i < arr.length - 1; i++) {
    result.push([arr[i], arr[i + 1]]);
  }
  return result;
};

const react = (polymer) => {
  let p = polymer.substring(0);
  let i = 0;
  while (i < p.length - 1) {
    if (p[i].toLowerCase() === p[i + 1].toLowerCase() && p[i] !== p[i + 1]) {
      p = p.substring(0, i) + p.substring(i + 2);
      i = 0;
      continue;
    }
    i++;
  }

  return p;
};

const run = (polymer) => react(polymer).length;

const partOne = (input) => {
  return run(input.join(''));
};

const partTwo = (polymer) => {
  const toRemove = new Set(polymer.map((x) => x.toLowerCase()));
  let min = Infinity;
  for (const ignore of toRemove.values()) {
    const input = polymer.filter((p) => p.toLowerCase() !== ignore);

    const result = run(input.join(''));
    if (result < min) {
      min = result;
    }
  }
  return min;
};

runner(parse, partOne, partTwo);
