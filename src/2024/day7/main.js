const runner = require('../../utils/runner');
const filterMap = require('../../utils/filterMap');

const parse = (input) =>
  filterMap(input.split('\n'), (line) => {
    if (line === '') return null;
    const [target, testValues] = line.split(': ');

    return {
      target: Number(target),
      values: testValues.split(' ').map(Number),
    };
  });

const add = (a, b) => a + b;
const mult = (a, b) => a * b;
const concat = (a, b) => Number(a.toString() + b.toString());

const combinations = (operators, numbers) => {
  const loop = (idx, value) => {
    if (idx === numbers.length) {
      return [value];
    }

    const next = numbers[idx];
    const result = [];

    for (const op of operators) {
      result.push(...loop(idx + 1, op(value, next)));
    }

    return result;
  };

  if (numbers.length === 0) return [];

  return loop(1, numbers[0]);
};

const valid = (operators) => (values, target) =>
  combinations(operators, values).includes(target);

const run = (valid) => (data) => {
  let count = 0;
  for (const { target, values } of data) {
    if (valid(values, target)) {
      count += target;
    }
  }
  return count;
};

const partOne = run(valid([add, mult]));
const partTwo = run(valid([add, mult, concat]));

runner(parse, partOne, partTwo);
