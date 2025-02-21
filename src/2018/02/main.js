const runner = require('../../utils/runner');
const parse = (input) =>
  input.split('\n').filter((x) => x !== '' && x !== '\n');

const partOne = (ids) => {
  let twos = 0;
  let threes = 0;
  for (const id of ids) {
    const arr = id.split('');
    let twoCount = 0;
    let threeCount = 0;
    const seen = new Set();
    for (let i = 0; i < arr.length; i++) {
      let count = 1;
      if (seen.has(arr[i])) continue;
      seen.add(arr[i]);
      for (let j = i + 1; j < arr.length; j++) {
        if (arr[i] === arr[j]) {
          count++;
        }
      }
      if (count === 2) twoCount++;
      if (count === 3) threeCount++;
    }

    if (twoCount > 0) twos++;
    if (threeCount > 0) threes++;
  }

  return twos * threes;
};

const union = (a, b) => {
  const result = [];
  for (let i = 0; i < a.length; i++) {
    if (a[i] === b[i]) {
      result.push(a[i]);
    }
  }
  return result;
};

const findTwoSimilarIds = (ids) => {
  for (let i = 0; i < ids.length; i++) {
    for (let j = i + 1; j < ids.length; j++) {
      const a = ids[i].split('');
      const b = ids[j].split('');
      const u = union(a, b);
      if (u.length + 1 === a.length) {
        return u.join('');
      }
    }
  }
};

const partTwo = (ids) => {
  return findTwoSimilarIds(ids);
};

runner(parse, partOne, partTwo);
