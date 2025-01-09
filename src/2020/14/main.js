const runner = require('../../utils/runner');

const parse = (input) => input.split('\n');

const parseMemLine = (line) => {
  const result = /^mem\[(\d+)\] = (\d+)$/.exec(line);

  if (result.length < 3) throw new Error('invalid line');

  const base10 = parseInt(result[2]);
  const base2 = base10.toString(2).padStart(36, '0');

  return {
    address: result[1],
    value: base2,
  };
};

const write = (value, mask) => {
  let result = value.split('');
  for (let i = 0; i < mask.length; i++) {
    if (mask[i] === 'X') continue;
    result[i] = mask[i];
  }
  return result.join('');
};

const partOne = (lines) => {
  const memory = {};
  let mask;
  for (const line of lines) {
    if (!line || line === '\n') continue;

    if (line.startsWith('mask = ')) {
      mask = line.split('mask = ')[1];
      continue;
    }

    const { address, value } = parseMemLine(line);

    if (!mask) throw new Error('no mask set');
    memory[address] = write(value, mask);
  }

  return Object.values(memory).reduce(
    (sum, value) => sum + parseInt(value, 2),
    0
  );
};

const combinations = (applied) => {
  const positions = applied.flatMap((x, idx) => (x === 'X' ? [idx] : []));
  const result = [];
  function backtrack(current, length) {
    if (length === positions.length) {
      result.push(current.slice());
      return;
    }
    // Add 0 and recurse
    current.push('0');
    backtrack(current, length + 1);
    current.pop();

    // Add 1 and recurse
    current.push('1');
    backtrack(current, length + 1);
    current.pop();
  }

  backtrack([], 0);

  return result.map((values) => {
    let idx = 0;
    return applied
      .map((x, j) => {
        if (j === positions[idx]) {
          const next = values[idx];
          idx++;
          return next;
        }

        return x;
      })
      .join('');
  });
};

const getCombinations = (value, mask) => {
  let applied = value.split('');
  for (let i = 0; i < mask.length; i++) {
    const bitmask = mask[i];
    if (bitmask === '0') continue;

    if (bitmask === '1') {
      applied[i] = '1';
    }

    if (bitmask === 'X') {
      applied[i] = 'X';
    }
  }

  return combinations(applied);
};

const partTwo = (lines) => {
  const memory = {};
  let mask;
  for (const line of lines) {
    if (!line || line === '\n') continue;

    if (line.startsWith('mask = ')) {
      mask = line.split('mask = ')[1];
      continue;
    }

    if (!mask) throw new Error('no mask set');
    const { address, value } = parseMemLine(line);

    const addresses = getCombinations(
      Number(address).toString(2).padStart(36, '0'),
      mask
    );
    for (const add of addresses) {
      const x = parseInt(add, 2);
      memory[x] = value;
    }
  }

  return Object.values(memory)
    .flat()
    .reduce((sum, value) => {
      const x = parseInt(value, 2);
      return sum + x;
    }, 0);
};

runner(parse, partOne, partTwo);
