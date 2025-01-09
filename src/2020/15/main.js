const runner = require('../../utils/runner');

const parse = (input) => {
  const [line] = input.split('\n');
  return line.split(',').map(Number);
};

const solve = (limit) => (input) => {
  let turns = new Map(input.map((value, i) => [value, i + 1]));
  let prev;
  let target = input[input.length - 1];
  for (let turn = input.length; turn < limit; turn++) {
    target = turns.has(target) ? turn - turns.get(target) : 0;
    turns.set(prev, turn);
    prev = target;
  }
  return target;
};

const partOne = solve(2020);
const partTwo = solve(30_000_000);

runner(parse, partOne, partTwo);
