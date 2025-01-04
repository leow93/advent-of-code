const runner = require('../../utils/runner');
const filterMap = require('../../utils/filterMap');

const parse = (input) => {
  const [patterns, designs] = input.split('\n\n');

  return {
    towels: patterns.split(', '),
    designs: filterMap(designs.split('\n'), (line) => {
      if (!line || line === '\n') return null;
      return line;
    }),
  };
};

const memo = (f, hash) => {
  const cache = new Map();
  return (...args) => {
    const key = hash(...args);
    if (cache.has(key)) return cache.get(key);

    const result = f(...args);
    cache.set(key, result);
    return result;
  };
};

const _isPossible = (towels, design) => {
  return (
    design === '' ||
    towels.some(
      (towel) =>
        design.startsWith(towel) &&
        isPossible(towels, design.substring(towel.length))
    )
  );
};

const isPossible = memo(_isPossible, (_, design) => design.toString());

const partOne = (data) => {
  let count = 0;
  for (let i = 0; i < data.designs.length; i++) {
    const design = data.designs[i];
    if (isPossible(data.towels, design)) count++;
  }
  return count;
};

function countWays(pattern, components) {
  const n = pattern.length;
  const dp = new Array(n + 1).fill(0);
  dp[0] = 1;

  for (let i = 1; i <= n; i++) {
    for (const c of components) {
      const len = c.length;
      if (i >= len && pattern.slice(i - len, i) === c) {
        dp[i] += dp[i - len];
      }
    }
  }

  return dp[n];
}

const partTwo = (data) => {
  let count = 0;
  for (let i = 0; i < data.designs.length; i++) {
    const design = data.designs[i];
    count += countWays(design, data.towels);
  }
  return count;
};
runner(parse, partOne, partTwo);
