const runner = require('../../utils/runner');

const parse = (x) => {
  const [ranges, ingredients] = x.split('\n\n');
  return {
    ranges: ranges.split('\n').map((line) => line.split('-').map(Number)),
    ingredients: ingredients.split('\n').map(Number),
  };
};
const partOne = ({ ranges, ingredients }) =>
  ingredients.reduce((count, ingredient) => {
    const isFresh = ranges.some(
      (r) => ingredient >= r[0] && ingredient <= r[1]
    );
    return count + (isFresh ? 1 : 0);
  }, 0);

const collapseRanges = (ranges) => {
  const sorted = ranges.slice();
  sorted.sort((a, b) => a[0] - b[0]);

  const result = [sorted[0]];

  for (let i = 1; i < sorted.length; i++) {
    const curr = sorted[i];
    const last = result[result.length - 1];

    const [currentStart, currentEnd] = curr;
    const [_, lastEnd] = last;

    if (currentStart <= lastEnd) {
      last[1] = Math.max(lastEnd, currentEnd);
    } else {
      result.push(curr);
    }
  }

  return result;
};

const partTwo = ({ ranges }) => {
  const collapsed = collapseRanges(ranges);
  return collapsed.reduce((sum, range) => sum + (range[1] - range[0] + 1), 0);
};
runner(parse, partOne, partTwo);
