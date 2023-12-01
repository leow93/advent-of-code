const fs = require('fs');

const data = fs.readFileSync('./data.txt', 'utf8').split('\n');

const partOne = data.reduce((acc, line) => {
  const ints = line.split('').map(Number).filter(Number.isInteger);
  const number = 10 * ints[0] + ints[ints.length - 1];
  return acc + number;
}, 0);

console.log('partOne', partOne);

const transformations = {
  one: 1,
  two: 2,
  three: 3,
  four: 4,
  five: 5,
  six: 6,
  seven: 7,
  eight: 8,
  nine: 9,
};

const wordsMap = {
  one: '1',
  two: '2',
  three: '3',
  four: '4',
  five: '5',
  six: '6',
  seven: '7',
  eight: '8',
  nine: '9',
};

/**
 *
 * @param {string} line
 */
function parseLine(line) {
  const words = Object.keys(wordsMap);

  let first = null,
    last = null;
  let i = 0;
  for (i = 0; i < line.length; i++) {
    const char = line[i];
    const int = parseInt(char);
    if (Number.isInteger(int)) {
      if (first === null) {
        first = int;
      }
      last = int;
    } else {
      // must be a word
      const nextChar = line[i + 1];
      if (!nextChar) {
        break;
      }
      const word = words.find((word) => word.startsWith(char + nextChar));
      if (word === undefined) {
        continue;
      }
      if (line.slice(i, i + word.length) !== word) {
        continue;
      }
      const number = transformations[word];
      if (number !== undefined) {
        if (first === null) {
          first = number;
        }
        last = number;
      }
    }
  }

  return 10 * first + last;
}

const partTwo = data.reduce((acc, line) => acc + parseLine(line), 0);

console.log('partTwo', partTwo);
