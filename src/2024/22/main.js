const runner = require('../../utils/runner');
const fm = require('../../utils/filterMap');

const parse = (input) =>
  fm(input.split('\n'), (x) => (x === '' || x === '\n' ? null : Number(x)));

const mix = (x, y) => {
  return (x ^ y) >>> 0;
};

const prune = (x) => x % 16777216;

const step1 = (x) => {
  const x1 = x * 64;
  const x2 = mix(x1, x);
  return prune(x2);
};

const step2 = (x) => {
  const x1 = Math.floor(x / 32);
  const x2 = mix(x1, x);
  return prune(x2);
};

const step3 = (x) => {
  const x1 = x * 2048;
  const x2 = mix(x1, x);
  return prune(x2);
};

const cache = new Map();
const memo = (f, hash) => {
  return (...args) => {
    const key = hash(...args);
    if (cache.has(key)) {
      return cache.get(key);
    }

    const result = f(...args);
    cache.set(key, result);
    return result;
  };
};

const _newSecretNumber = (x) => {
  const result = step3(step2(step1(x)));
  return result;
};
const newSecretNumber = memo(_newSecretNumber, (x) => x.toString());

const nthNewSecretNumber = (n) => (x) => {
  let result = x;
  for (let i = 0; i < n; i++) {
    result = newSecretNumber(result);
  }
  return result;
};

const sum = (x, y) => x + y;

const partOne = (data) => {
  const f = nthNewSecretNumber(2000);
  return data.map(f).reduce(sum, 0);
};
const partTwo = (data) => 'todo';

runner(parse, partOne, partTwo).then(() => {
  console.log(cache);
});
