const runner = require('../../utils/runner');
const filterMap = require('../../utils/filterMap');

const combinations = (...arrays) => {
  const combineTwoArrays = (xs, ys) => {
    const result = new Set();
    for (let i = 0; i < xs.length; i++) {
      for (let j = 0; j < ys.length; j++) {
        result.add(xs[i] + ys[j]);
        result.add(ys[j] + xs[i]);
      }
    }

    return Array.from(result);
  };

  const result = arrays.reduce((combined, arr) =>
    combineTwoArrays(combined, arr)
  );
  return result;
};

const parseRules = (rules) => {
  const map = {};
  const queue = [...rules];
  while (queue.length > 0) {
    const curr = queue.shift();
    if (!curr || curr === '\n') continue;

    const [key, rule] = curr.split(': ');

    if (rule.startsWith('"') && rule.endsWith('"') && rule.length === 3) {
      map[key] = [rule[1]];
      continue;
    }

    const options = rule.split(' | ');
    if (options.length !== 2 && options.length !== 1) {
      throw new Error('only up to two options supported');
    }

    for (const option of options) {
      const parts = option.split(' ');
      map[key] = map[key] ?? [];

      if (parts.some((p) => !(p in map) || map[p]?.length === 0)) {
        // can't process yet, enqueue
        queue.push(curr);
        break;
      }

      const combos = combinations(...parts.map((p) => map[p]));

      map[key].push(...combos);
    }
  }
  return map;
};

const parse = (input) => {
  const [rules, inputs] = input.split('\n\n');

  return {
    rules: parseRules(rules.split('\n')),
    inputs: filterMap(inputs.split('\n'), (line) =>
      !line || line === '\n' ? null : line
    ),
  };
};

const valid = (rules, input) => {
  return input === buildPattern(rules, rules[0]);
};

const partOne = (data) => {
  console.log(JSON.stringify(data.rules, null, 2));
  let count = 0;

  for (const input of data.inputs) {
    if (valid(data.rules, input)) {
      count++;
    }
  }

  return count;
};
const partTwo = (data) => 'todo';

runner(parse, partOne, partTwo);
