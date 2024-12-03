const readFromStdin = require('../../utils/readFromStdin');

const parseRule = (rule) => {
  const regexp = /^(\d+)-(\d+) ([a-z]{1})$/i;
  const match = regexp.exec(rule);
  if (match.length < 4) {
    return null;
  }

  return {
    min: parseInt(match[1]),
    max: parseInt(match[2]),
    char: match[3],
  };
};

const parse = (input) => {
  const [rule, password] = input.split(': ');

  return {
    password,
    rule: parseRule(rule),
  };
};

const filterMap = (f) => (xs) => {
  const ys = [];
  for (const x of xs) {
    const y = f(x);
    if (y != null) {
      ys.push(y);
    }
  }
  return ys;
};

const each =
  (...fs) =>
  (...args) =>
    fs.map((f) => f(...args));

const charCount = ({ password, rule }) => {
  let count = 0;
  const { max, min, char } = rule;
  for (let i = 0; i < password.length; ++i) {
    if (char === password[i]) {
      count++;
    }
  }

  return min <= count && count <= max;
};

const position = ({ password, rule }) => {
  const { max, min, char } = rule;
  const start = min - 1;
  const end = max - 1;

  return (
    (password[start] === char && password[end] !== char) ||
    (password[end] === char && password[start] !== char)
  );
};

const countBy = (f) => (xs) => {
  let count = 0;
  for (const x of xs) {
    if (f(x)) count++;
  }
  return count;
};

const partOne = countBy(charCount);

const partTwo = countBy(position);

const main = () =>
  readFromStdin()
    .then((s) => s.split('\n'))
    .then(filterMap(parse))
    .then(each(partOne, partTwo))
    .then(console.log);

main();
