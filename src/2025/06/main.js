const readFromStdIn = require('../../utils/readFromStdin');

const add = (x, y) => x + y;
const mul = (x, y) => x * y;

const partOne = (input) => {
  const lines = input.split('\n').filter(Boolean);
  const problems = [];

  for (let i = 0; i < lines.length; i++) {
    const line = lines[i];
    const parts = line.split(/\s+/).filter(Boolean);

    for (let j = 0; j < parts.length; j++) {
      problems[j] = problems[j] ?? [];
      problems[j][i] = parts[j];
    }
  }

  return problems.reduce((sum, problem) => {
    const op = problem[problem.length - 1] === '+' ? add : mul;

    return (
      sum +
      problem
        .slice(0, problem.length - 1)
        .map(Number)
        .reduce(op)
    );
  }, 0);
};

const partTwo = (input) => {
  const lines = input.split('\n').filter(Boolean);
  const grid = lines.map((l) => l.split(''));
  const width = grid[0].length;

  const problems = [];
  let nums = [];
  for (let i = width - 1; i >= 0; i--) {
    const num = [];
    let op;
    for (let j = 0; j < lines.length; j++) {
      const line = lines[j];
      const n = line[i];
      if (n === '*') {
        op = mul;
      } else if (n === '+') {
        op = add;
      } else if (n !== ' ') {
        num.push(n);
      }
    }

    if (num.length) {
      nums.push(Number(num.join('')));
    }
    if (op) {
      problems.push({ nums, op });
      nums = [];
    }
  }

  return problems.reduce((sum, p) => sum + p.nums.reduce(p.op), 0);
};

const main = async () => {
  const input = await readFromStdIn();
  console.log([partOne(input), partTwo(input)]);
};

void main();
