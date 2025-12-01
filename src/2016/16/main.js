const runner = require('../../utils/runner');

const parse = (data) => data;

const rev = (str) => {
  let result = '';
  for (let i = str.length - 1; i >= 0; i--) {
    result += str[i];
  }
  return result;
};

const swap = (str) => {
  let result = '';
  for (let i = 0; i < str.length; i++) {
    result += str[i] === '0' ? '1' : '0';
  }
  return result;
};

const process = (str) => {
  const a = str;
  const b = swap(rev(a));

  return a + '0' + b;
};

const toTuples = (str) => {
  const result = [];
  for (let i = 0; i < str.length - 1; i += 2) {
    result.push([str[i], str[i + 1]]);
  }

  return result;
};

const checksum = (str) => {
  const pairs = toTuples(str);
  const cs = pairs.map(([a, b]) => (a === b ? '1' : '0')).join('');
  if (cs.length % 2 === 0) {
    return checksum(cs);
  }

  return cs;
};

const DISK_SIZE = 272;
const DISK_SIZE_TWO = 35651584;

const run = (diskSize) => (input) => {
  let x = input;

  while (x.length <= diskSize) {
    x = process(x);
  }

  x = x.slice(0, diskSize);

  return checksum(x);
};

const partOne = run(DISK_SIZE);
const partTwo = run(DISK_SIZE_TWO);

runner(parse, partOne, partTwo);
