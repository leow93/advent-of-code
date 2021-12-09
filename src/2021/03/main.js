const fs = require('fs');
const path = require('path');

function parseFile(filename) {
  return fs
    .readFileSync(path.resolve(__dirname, filename))
    .toString()
    .split('\n')
    .map((line) => line.split('').map(Number));
}

function transpose(matrix) {
  return Array.from({ length: matrix[0].length }).map((_, i) =>
    Array.from({ length: matrix.length }).map((_, j) => matrix[j][i])
  );
}

function countOccurrences(list) {
  return list.reduce(
    (map, item) => {
      if (!(item in map)) {
        throw new Error('Unknown bit: ' + item);
      }

      return Object.assign({}, map, { [item]: map[item] + 1 });
    },
    { 0: 0, 1: 0 }
  );
}

function partOne(lines) {
  const occurrences = transpose(lines).map(countOccurrences);
  const gamma = parseInt(
    occurrences.map((item) => (item[0] > item[1] ? 0 : 1)).join(''),
    2
  );
  const epsilon = parseInt(
    occurrences.map((item) => (item[0] > item[1] ? 1 : 0)).join(''),
    2
  );
  return gamma * epsilon;
}

function partTwo(lines) {
  // const transposed = transpose(lines);
  // const result = transposed;
  // for (let i = 0; i < transposed.length; i++) {
  //   const
  // }

  const calculate = (filter, arr, i) => {
    if (arr.length === 1) {
      return arr[0];
    }

    const [zeroes, ones] = arr.reduce(
      (state, item) => {
        if (item[i] === 0) {
          return [state[0] + 1, state[1]];
        }

        return [state[0], state[1] + 1];
      },
      [0, 0]
    );

    const filtered = arr.filter(filter(zeroes, ones, i));

    return calculate(filter, filtered, i + 1);
  };

  const o2Filter = (zeroes, ones, i) => (digits) =>
    (zeroes > ones && digits[i] === 0) || (zeroes <= ones && digits[i] === 1);
  const co2Filter = (zeroes, ones, i) => (digits) =>
    (zeroes <= ones && digits[i] === 0) || (ones < zeroes && digits[i] === 1);

  return (
    parseInt(calculate(o2Filter, lines, 0).join(''), 2) *
    parseInt(calculate(co2Filter, lines, 0).join(''), 2)
  );
}

function main() {
  console.log('Part I test: ', partOne(parseFile('test.txt')));
  console.log('Part I: ', partOne(parseFile('data.txt')));

  console.log('Part II test: ', partTwo(parseFile('test.txt')));
  console.log('Part II: ', partTwo(parseFile('data.txt')));
}

main();
