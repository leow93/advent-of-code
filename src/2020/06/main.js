const runner = require('../../utils/runner');

const parse = (input) => input.split('\n\n').map((line) => line.split('\n'));

const partOne = (groups) => {
  let count = 0;
  for (const g of groups) {
    const answers = new Set();
    for (const p of g) {
      for (const a of p) {
        answers.add(a);
      }
    }

    count += answers.size;
  }
  return count;
};

class Counter {
  data = new Map();
  inc(key) {
    if (this.data.has(key)) {
      this.data.set(key, this.data.get(key) + 1);
      return;
    }
    this.data.set(key, 1);
  }
  filterByValue(x) {
    const keys = [];
    for (const [k, v] of this.data.entries()) {
      if (v === x) {
        keys.push(k);
      }
    }
    return keys;
  }
}

const partTwo = (groups) => {
  let count = 0;
  for (const group of groups) {
    const counter = new Counter();
    for (const person of group) {
      for (const answer of person) {
        counter.inc(answer);
      }
    }
    const questionsAnsweredByAll = counter.filterByValue(group.length);

    count += questionsAnsweredByAll.length;
  }
  return count;
};

runner(parse, partOne, partTwo);
