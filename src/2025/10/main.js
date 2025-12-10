const solver = require('javascript-lp-solver');
const runner = require('../../utils/runner');

const parseIndicators = (str) => {
  const lights = str.slice(1, str.length - 1);

  return {
    initialState: new Array(lights.length).fill(0),
    required: Array.from({ length: lights.length }).map((_, i) =>
      lights[i] === '.' ? 0 : 1
    ),
  };
};
const parseJoltage = (str) => {
  const js = str.slice(1, str.length - 1);
  return js.split(',').map(Number);
};
const parseButtons = (buttons, length) => {
  return buttons.map((b) => {
    return b
      .slice(1, b.length - 1)
      .split(',')
      .map(Number);
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
const pushButton = (state, button) => {
  const result = state.slice();
  for (const i of button) {
    result[i] = 1 - result[i];
  }
  return result;
};
const eq = (xs, ys) => {
  if (xs.length !== ys.length) return false;
  return xs.every((x, i) => x === ys[i]);
};
const fewestButtonCount = (machine, id) => {
  const { buttons, initialState, required } = machine;
  let c = 1;
  while (true) {
    const combos = combinationsOfSize(buttons, c);
    for (const buttons of combos) {
      const result = buttons.reduce(pushButton, initialState);
      if (eq(result, required)) {
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

function solveILP(targetVector, instructions) {
  const model = {
    optimize: 'totalCost', // We want to minimize this variable
    opType: 'min',
    constraints: {},
    variables: {},
    ints: {}, // Forces variables to be Integers
  };

  // 2. Define Constraints (The Target Vector)
  // Each index in the target vector is a constraint equation:
  // Inst_A * A_i + Inst_B * B_i ... = Target_i
  targetVector.forEach((val, idx) => {
    model.constraints[`c_${idx}`] = { equal: val };
  });

  instructions.forEach((instrIndices, i) => {
    const varName = `instr_${i}`;

    // This variable contributes 1 to our "totalCost" (count of ops)
    const variable = { totalCost: 1 };

    // This variable affects specific constraints (vector indices)
    instrIndices.forEach((targetIdx) => {
      variable[`c_${targetIdx}`] = 1;
    });

    model.variables[varName] = variable;

    // Enforce Integer constraint
    model.ints[varName] = 1;
  });

  const results = solver.Solve(model);

  if (!results.feasible) {
    throw new Error('not possible');
  }
  return results.result;
}

const partTwo = (machines) =>
  machines.reduce(
    (count, machine, i) => count + solveILP(machine.joltage, machine.buttons),
    0
  );

runner(parse, partOne, partTwo);
