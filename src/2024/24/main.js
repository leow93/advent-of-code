const runner = require('../../utils/runner');
const fm = require('../../utils/filterMap');

const parseWires = (wires) => {
  const lines = wires.split('\n');

  const map = new Map();
  for (const line of lines) {
    if (!line || line === '\n') continue;
    const [id, value] = line.split(': ');
    map.set(id, Number(value));
  }
  return map;
};

const getOp = (type) => {
  switch (type) {
    case 'AND':
      return (a, b) => (a && b ? 1 : 0);
    case 'OR':
      return (a, b) => (a || b ? 1 : 0);
    case 'XOR':
      return (a, b) => (a !== b ? 1 : 0);
    default:
      throw new Error('Unknown operator: ' + type);
  }
};

const parseGates = (gates) => {
  return fm(gates.split('\n'), (line) => {
    if (!line || line === '\n') return null;

    const [inputs, output] = line.split(' -> ');

    const [a, type, b] = inputs.split(' ');

    return {
      a,
      b,
      op: getOp(type),
      c: output,
    };
  });
};

const parse = (input) => {
  const [wires, gates] = input.split('\n\n');

  return {
    wires: parseWires(wires),
    gates: parseGates(gates),
  };
};

const run = (data) => {
  const { wires, gates } = data;
  const queue = [...gates];

  while (queue.length) {
    console.log(queue.length);
    const { a, b, c, op } = queue.shift();

    // need both inputs to run the operation
    if (!wires.has(a) || !wires.has(b)) {
      queue.push({ a, b, c, op });
      continue;
    }

    const result = op(wires.get(a), wires.get(b));
    wires.set(c, result);
  }

  return wires;
};

const partOne = (data) => {
  const wires = run(data);
  const zs = [];
  for (const k of wires.keys()) {
    if (k.startsWith('z')) zs.push(k);
  }
  const bits = zs
    .sort()
    .reverse()
    .map((x) => wires.get(x))
    .join('');

  return parseInt(bits, 2);
};
const partTwo = (data) => {};

runner(parse, partOne, partTwo);
