const runner = require('../../utils/runner');
const filterMap = require('../../utils/filterMap');

const instruction = (id, type, value) => ({
  id,
  type,
  value,
});

const parse = (input) =>
  filterMap(input.split('\n'), (line, i) => {
    if (!line) return null;

    const [instr, value] = line.split(' ');
    return instruction(i, instr, Number(value));
  });

const partOne = (instructions) => {
  const seen = new Set();
  const queue = [instructions[0]];
  let acc = 0;

  while (queue.length) {
    const instr = queue.shift();
    if (seen.has(instr.id)) {
      return { exit: 1, acc };
    }
    seen.add(instr.id);
    switch (instr.type) {
      case 'nop': {
        const next = instructions[instr.id + 1];
        if (next) {
          queue.push(next);
        }
        break;
      }
      case 'acc': {
        acc += instr.value;
        const next = instructions[instr.id + 1];
        if (next) {
          queue.push(next);
        }
        break;
      }
      case 'jmp': {
        const next = instructions[instr.id + instr.value];
        if (next) {
          queue.push(next);
        }
        break;
      }
    }
  }

  return { exit: 0, acc };
};

const partTwo = (instructions) => {
  const runs = filterMap(instructions, (instr, i) => {
    if (instr.type === 'acc') return null;
    const replaced = instruction(
      i,
      instr.type === 'nop' ? 'jmp' : 'nop',
      instr.value
    );
    return [
      ...instructions.slice(0, i),
      replaced,
      ...instructions.slice(i + 1),
    ];
  });

  for (const instructions of runs) {
    const result = partOne(instructions);
    if (result.exit === 0) {
      return result.acc;
    }
  }

  throw new Error('should have exited cleanly');
};

runner(parse, partOne, partTwo);
