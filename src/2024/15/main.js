const readFromStdin = require('../../utils/readFromStdin');
const partOne = require('./part1');
const partTwo = require('./part2');

async function main() {
  const input = await readFromStdin();
  console.log('Part I:', partOne(input));
  console.log('Part II:', partTwo(input));
}

void main();
