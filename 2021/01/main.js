const fs = require('fs');
const path = require('path');

function readFile(filename) {
  return fs.readFileSync(path.resolve(__dirname + filename)).toString();
}

function parseFile(filename) {
  return readFile(filename).trim().split('\n').map(Number);
}

function countIncreases(list) {
  let count = 0;
  for (let i = 1; i < list.length; ++i) {
    if (list[i] > list[i - 1]) {
      count++;
    }
  }

  return count;
}

function partOne() {
  console.log('Part I:', countIncreases(parseFile('/data.txt')));
}

function sum(...numbers) {
  return numbers.reduce((x, n) => x + n, 0);
}

function windowSumIncrease(list) {
  let count = 0;
  let previousWindow;

  for (let i = 0; i < list.length - 2; i++) {
    const window = [list[i], list[i + 1], list[i + 2]];
    const total = sum(...window);
    if (previousWindow !== undefined && total > previousWindow) {
      count++;
    }
    previousWindow = total;
  }

  return count;
}

function partTwo() {
  console.log('Part II:', windowSumIncrease(parseFile('/data.txt')));
}

function main() {
  partOne();
  partTwo();
}

main();
