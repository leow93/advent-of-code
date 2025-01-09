const filterMap = require('../../utils/filterMap');
const runner = require('../../utils/runner');

const sort = (a, b) => a - b;

const parse = (input) => {
  const sortedInput = filterMap(input.split('\n'), (x) =>
    x === '' || x === '\n' ? null : Number(x)
  ).sort(sort);

  return [0, ...sortedInput, sortedInput[sortedInput.length - 1] + 3];
};

const partOne = (ratings) => {
  let threeJoltJumps = 0;
  let oneJoltJumps = 0;

  for (let i = 0; i < ratings.length - 1; i++) {
    const diff = ratings[i + 1] - ratings[i];

    if (diff === 1) oneJoltJumps++;
    if (diff === 3) threeJoltJumps++;
  }

  return threeJoltJumps * oneJoltJumps;
};

const partTwo = (ratings) => {
  const stepCounter = { 0: 1 };
  for (let i = 0; i < ratings.length; i++) {
    let j = i + 1;
    while (ratings[j] <= ratings[i] + 3) {
      stepCounter[j] = (stepCounter[j] || 0) + stepCounter[i];
      j++;
    }
  }
  return stepCounter[ratings.length - 1];
};

runner(parse, partOne, partTwo);
