const input = require('fs').readFileSync('./data.txt').toString();

function* combinations(arr, size = arr.length) {
  const end = arr.length - 1;
  const combination = [];

  function* recur(start, index) {
    if (index === size) {
      return yield combination;
    }

    for (let i = start; i <= end && end - i + 1 >= size - index; i++) {
      combination[index] = arr[i];
      yield* recur(i + 1, index + 1);
    }
  }

  yield* recur(0, 0);
}

const parsePackageWeights = (input) => input.split('\n').map(Number);

const groupsOfWeight = function* (packages, weightPerGroup) {
  for (let groupSize = 1; groupSize <= packages.length; groupSize++) {
    for (const group of combinations(packages, groupSize)) {
      if (group.reduce(sum, 0) === weightPerGroup) {
        yield group;
      }
    }
  }
};

const sum = (a, b) => a + b;
const product = (a, b) => a * b;

const sub = (xs, ys) => xs.filter((x) => !ys.includes(x));

const canGroup = (packages, numOfGroups, weightPerGroup) => {
  if (numOfGroups === 0) return packages.length === 0;

  for (const group of groupsOfWeight(packages, weightPerGroup)) {
    if (canGroup(sub(packages, group), numOfGroups - 1, weightPerGroup)) {
      return true;
    }
  }

  return false;
};

const idealFirstGroupQE = (packages, numOfGroups) => {
  const weightPerGroup = packages.reduce(sum, 0) / numOfGroups;

  let minQE = Infinity;
  let prevGroupSize = Infinity;

  for (const group of groupsOfWeight(packages, weightPerGroup)) {
    if (minQE !== Infinity && prevGroupSize < group.length) break;

    const candidateQE = group.reduce(product, 1);
    if (
      candidateQE < minQE &&
      canGroup(sub(packages, group), numOfGroups - 1, weightPerGroup)
    ) {
      minQE = candidateQE;
    }

    prevGroupSize = group.length;
  }

  return minQE;
};
const part1 = (input) => idealFirstGroupQE(parsePackageWeights(input), 3);
const part2 = (input) => idealFirstGroupQE(parsePackageWeights(input), 4);

console.log('Part I: ', part1(input));
console.log('Part II: ', part2(input));

function clean(s) {
  return s.replace(/Rn/g, '(').replace(/Ar/g, ')').replace(/Y/g, ',');
}
