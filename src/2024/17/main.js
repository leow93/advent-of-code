const runner = require('../../utils/runner');

const getRegister = (registers, code) => {
  const reg = registers
    .split('\n')
    .find((x) => x.startsWith(`Register ${code}: `));
  if (!reg) throw new Error('register not found ' + code);
  const value = reg.split(`Register ${code}: `)[1];
  return Number(value);
};

const parse = (input) => {
  const [registers, program] = input.split('\n\n');

  return {
    A: getRegister(registers, 'A'),
    B: getRegister(registers, 'B'),
    C: getRegister(registers, 'C'),
    input: program.split('Program: ')[1].split(',').map(Number),
  };
};

const run = (data) => {
  let insPtr = 0;
  let a = data.A;
  let b = data.B;
  let c = data.C;

  const literal = (x) => x;
  const combo = (x) => {
    if (x <= 3) return x;
    if (x === 4) return a;
    if (x === 5) return b;
    if (x === 6) return c;
    throw new Error('unxpected combo: ' + x);
  };

  const out = [];

  while (true) {
    const instruction = data.input[insPtr];
    const operand = data.input[insPtr + 1];
    if (instruction === undefined || operand === undefined) {
      break;
    }
    let jumped = false;
    switch (instruction) {
      case 0: {
        // adv
        const numerator = a;
        const denominator = Math.pow(2, combo(operand));
        const result = Math.floor(numerator / denominator);
        a = result;
        break;
      }
      case 1: {
        // bxl
        const result = b ^ literal(operand);
        b = result;
        break;
      }
      case 2: {
        // bst
        const result = combo(operand) % 8;
        b = result;
        break;
      }
      case 3: {
        // jnz
        if (a === 0) {
          break;
        }
        insPtr = literal(operand);
        jumped = true;
        break;
      }
      case 4: {
        // bxc
        const result = b ^ c;
        b = result;
        break;
      }
      case 5: {
        // out
        const result = combo(operand) % 8;
        out.push(result);
        break;
      }
      case 6: {
        // bdv
        const numerator = a;
        const denominator = Math.pow(2, combo(operand));
        const result = Math.floor(numerator / denominator);
        b = result;
        break;
      }
      case 7: {
        // cdv
        const numerator = a;
        const denominator = Math.pow(2, combo(operand));
        const result = Math.floor(numerator / denominator);
        c = result;
        break;
      }
    }
    if (!jumped) {
      insPtr += 2;
    }
  }

  return out.join(',');
};

const partOne = run;
const partTwo = (data) => {
  const input = data.input.join(',');

  console.log(input);
  const a = 8 ** 15;

  console.log(run({ ...data, A: a }));
  console.log(run({ ...data, A: a + 8 ** 15 }));
  console.log(run({ ...data, A: a + 8 ** 14 }));
  // while (true) {
  //   const result = run({ ...data, A: a });
  //   if (result === input) {
  //     return a;
  //   }
  //   a += 1;
  // }
};

runner(parse, partOne, partTwo);
