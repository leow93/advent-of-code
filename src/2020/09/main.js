const runner = require('../../utils/runner');
const filterMap = require('../../utils/filterMap');

const parse = (input) =>
  filterMap(input.split('\n'), (line) => {
    if (!line) return null;
    return Number(line);
  });

const preambleLength = 25;

const isSumOfAnyPrevious = (n, prev) => {
  for (let i = 0; i < prev.length - 1; i++) {
    for (let j = i + 1; j < prev.length; j++) {
      if (prev[i] + prev[j] === n) {
        return true;
      }
    }
  }
  return false;
};

const partOne = (numbers) => {
  const loop = (idx) => {
    if (idx >= numbers.length) {
      return null;
    }
    const n = numbers[idx];
    const prev = numbers.slice(idx - preambleLength, idx);
    if (!isSumOfAnyPrevious(n, prev)) {
      return n;
    }

    return loop(idx + 1);
  };

  return loop(preambleLength);
};

const findAddingSet = (numbers, n) => {
  for (let i = 0; i < numbers.length; i++) {
    let buf = [numbers[i]];
    let sum = numbers[i];
    let idx = i + 1;
    while (sum < n && idx < numbers.length) {
      sum += numbers[idx];
      buf.push(numbers[idx]);
      idx++;
    }
    if (sum === n) {
      return buf;
    }
  }
  return null;
};

const partTwo = (numbers) => {
  const n = partOne(numbers);
  const set = findAddingSet(numbers, n);
  return Math.min(...set) + Math.max(...set);
};

runner(parse, partOne, partTwo);
