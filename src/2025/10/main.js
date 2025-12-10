const runner = require('../../utils/runner');
const parseIndicators = (str) => {
  const lights = str.slice(1, str.length - 1);

  return {
    initialState: 0,
    required: parseInt(
      Array.from({ length: lights.length })
        .map((_, i) => (lights[i] === '.' ? '0' : '1'))
        .join(''),
      2
    ),
    length: lights.length,
  };
};
const parseJoltage = (str) => {
  const js = str.slice(1, str.length - 1);
  return js.split(',').map(Number);
};
const parseButtons = (buttons, length) => {
  return buttons.map((b) => {
    const indices = b
      .slice(1, b.length - 1)
      .split(',')
      .map(Number);
    let mask = 0;
    for (let i = 0; i < indices.length; i++) {
      // Invert the index:
      // Index 0 shifts to the far left (length - 1)
      // Index 4 shifts to the far right (0)
      const shift = length - 1 - indices[i];

      mask |= 1 << shift;
    }
    return mask;
  });
};
const parseMachine = (line) => {
  const parts = line.split(' ');
  const { initialState, required, length } = parseIndicators(parts[0]);
  return {
    initialState,
    required,
    joltage: parseJoltage(parts.at(-1)),
    buttons: parseButtons(parts.slice(1, parts.length - 1), length),
  };
};

const parse = (x) => x.split('\n').filter(Boolean).map(parseMachine);

function combinationsOfSize(arr, k) {
  const result = [];
  const current = [];

  function backtrack(start) {
    if (current.length === k) {
      result.push([...current]);
      return;
    }

    for (let i = start; i < arr.length; i++) {
      current.push(arr[i]);
      backtrack(i); // allow reuse of the same element
      current.pop();
    }
  }

  backtrack(0);
  return result;
}
const fewestButtonCount = (machine, id) => {
  const { buttons, initialState, required } = machine;
  let c = 1;
  while (true) {
    const combos = combinationsOfSize(buttons, c);
    for (const buttons of combos) {
      const result = buttons.reduce((acc, b) => (acc ^= b), initialState);
      if (result === required) {
        return buttons.length;
      }
    }
    c++;
  }
};

const partOne = (machines) =>
  machines.reduce(
    (count, machine, i) => count + fewestButtonCount(machine, i),
    0
  );
const partTwo = () => {};

runner(parse, partOne, partTwo);
