const runner = require('../../utils/runner');

const parse = (input) => input.split('\n').filter((x) => x && x !== '\n');
const evaluateLeftToRight = (tokens) => {
  let result = parseInt(tokens[0], 10);
  for (let i = 1; i < tokens.length; i += 2) {
    const operator = tokens[i];
    const nextValue = parseInt(tokens[i + 1], 10);

    if (operator === '+') {
      result += nextValue;
    } else if (operator === '*') {
      result *= nextValue;
    }
  }
  return result;
};

const evaluateAdditionFirst = (tokens) => {
  while (tokens.includes('+')) {
    for (let i = 0; i < tokens.length; i++) {
      if (tokens[i] === '+') {
        const sum = parseInt(tokens[i - 1]) + parseInt(tokens[i + 1]);
        tokens.splice(i - 1, 3, sum); // Replace the operation with its result
        break; // Restart loop to find next '+'
      }
    }
  }

  // Resolve multiplications left-to-right
  let result = parseInt(tokens[0]);
  for (let i = 1; i < tokens.length; i += 2) {
    const nextValue = parseInt(tokens[i + 1]);
    result *= nextValue;
  }
  return result;
};

const execute = (input, evaluate) => {
  const stack = [];
  let current = [];
  for (const char of input.replace(/\s+/g, '')) {
    if (char === '(') {
      // Push the current expression onto the stack and start a new one
      stack.push(current);
      current = [];
    } else if (char === ')') {
      // Evaluate the current expression and add the result to the last expression on the stack
      const value = evaluate(current);
      current = stack.pop();
      current.push(value.toString());
    } else if (['+', '*'].includes(char) || !isNaN(char)) {
      // Add numbers and operators to the current expression
      current.push(char);
    }
  }
  return evaluate(current);
};

const run = (evaluate) => (lines) => {
  return lines.reduce((sum, line) => sum + execute(line, evaluate), 0);
};

const partOne = run(evaluateLeftToRight);
const partTwo = run(evaluateAdditionFirst);

runner(parse, partOne, partTwo);
