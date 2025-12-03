const assert = require('node:assert');
const runner = require('../../utils/runner');

const parse = (x) => x.split('\n').map((line) => line.split('').map(Number));

const maxJoltageNBatteries = (n) => (bank) => {
  let prevIdx = -1;
  const number = new Array(n).fill(0);

  for (let idx = 0; idx < n; idx++) {
    for (let i = prevIdx + 1; i <= bank.length - n + idx; i++) {
      if (bank[i] > number[idx]) {
        prevIdx = i;
        number[idx] = bank[i];
      }
    }
  }

  return Number(number.join(''));
};

const run = (maxJoltage) => (banks) =>
  banks.reduce((sum, bank) => sum + maxJoltage(bank), 0);

const partOne = run(maxJoltageNBatteries(2));
const partTwo = run(maxJoltageNBatteries(12));

runner(parse, partOne, partTwo);
