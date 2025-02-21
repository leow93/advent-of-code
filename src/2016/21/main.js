const runner = require('../../utils/runner');

const actions = {
  swapPosition: (x, y) => ({
    type: 'swap_position',
    x,
    y,
  }),
  swapLetter: (x, y) => ({
    type: 'swap_letter',
    x,
    y,
  }),

  rotateAtLetter: (x) => ({
    type: 'rotate_at_letter',
    x,
  }),

  rotateRight: (n) => ({
    type: 'rotate_right',
    n,
  }),
  rotateLeft: (n) => ({
    type: 'rotate_left',
    n,
  }),

  reversePositions: (x, y) => ({
    type: 'reverse_positions',
    x,
    y,
  }),
  movePositions: (x, y) => ({
    type: 'move_positions',
    x,
    y,
  }),
};

const rotRight = (str, n = 1) => {
  const len = str.length;
  const idxs = Array.from({ length: str.length }).map((_, i) => {
    const idx = i - n;
    return ((idx % len) + len) % len;
  });

  let result = '';
  for (let i = 0; i < idxs.length; i++) {
    result += str[idxs[i]];
  }
  return result;
};

const rotLeft = (str, n = 1) => {
  const len = str.length;
  const idxs = Array.from({ length: str.length }).map((_, i) => {
    const idx = i + n;
    return ((idx % len) + len) % len;
  });

  let result = '';
  for (let i = 0; i < idxs.length; i++) {
    result += str[idxs[i]];
  }
  return result;
};

const parseLine = (line) => {
  if (line.startsWith('swap position')) {
    const reg = /.+(\d+).+(\d+)/;
    const match = reg.exec(line);
    return actions.swapPosition(Number(match[1]), Number(match[2]));
  }

  if (line.startsWith('swap letter')) {
    const x = line.split('swap letter ')[1];
    return actions.swapLetter(x[0], x[x.length - 1]);
  }

  if (line.startsWith('rotate based on position of letter ')) {
    return actions.rotateAtLetter(line[line.length - 1]);
  }

  if (line.startsWith('rotate right ')) {
    return actions.rotateRight(Number(line.split('rotate right ')[1][0]));
  }

  if (line.startsWith('rotate left')) {
    return actions.rotateLeft(Number(line.split('rotate left ')[1][0]));
  }

  if (line.startsWith('reverse positions ')) {
    const x = line.split('reverse positions ')[1];
    return actions.reversePositions(Number(x[0]), Number(x[x.length - 1]));
  }

  if (line.startsWith('move position ')) {
    const x = line.split('move position ')[1];
    return actions.movePositions(Number(x[0]), Number(x[x.length - 1]));
  }
  throw new Error('unknown command: ' + line);
};

const parse = (input) =>
  input
    .split('\n')
    .filter((x) => x && x !== '\n')
    .map(parseLine);

const reducer =
  (reverse = false) =>
  (s, command) => {
    const result = s.slice().split('');
    switch (command.type) {
      case 'swap_position': {
        const x = reverse ? command.y : command.x;
        const y = reverse ? command.x : command.y;
        result[x] = s[y];
        result[y] = s[x];
        return result.join('');
      }
      case 'swap_letter': {
        const x = reverse ? command.y : command.x;
        const y = reverse ? command.x : command.y;

        let i, j;
        for (let k = 0; k < s.length; k++) {
          const ch = s[k];
          if (ch === x) {
            i = k;
          }
          if (ch === y) {
            j = k;
          }
        }
        result[i] = s[j];
        result[j] = s[i];
        return result.join('');
      }

      case 'rotate_right': {
        const { n } = command;
        return reverse
          ? rotLeft(result.join(''), n)
          : rotRight(result.join(''), n);
      }
      case 'rotate_left': {
        const { n } = command;
        return reverse
          ? rotRight(result.join(''), n)
          : rotLeft(result.join(''), n);
      }

      case 'rotate_at_letter': {
        const { x } = command;
        const idx = result.findIndex((c) => c === x);
        const rotations = idx + 1 + (idx >= 4 ? 1 : 0);
        const rot = reverse ? rotLeft : rotRight;
        return rot(result.join(''), rotations);
      }

      case 'reverse_positions': {
        const x = command.x;
        const y = command.y;

        const before = s.slice(0, x);
        const after = s.slice(y + 1);
        const between = s.slice(x, y + 1);
        return before + between.split('').reverse().join('') + after;
      }

      case 'move_positions': {
        const x = command.x;
        const y = command.y;

        const [letter] = result.splice(x, 1);
        result.splice(y, 0, letter);
        return result.join('');
      }

      default:
        throw new Error('unknown command: ' + command.type);
    }
  };

const PWD = 'abcde';

const partOne = (commands) => {
  return commands.reduce(reducer(false), PWD);
};
const partTwo = (commands) => {
  const red = reducer(true);
  return commands.reverse().reduce((state, cmd) => {
    const result = red(state, cmd);
    console.log({
      given: state,
      cmd,
      result,
    });
    return result;
  }, 'decab');
};

runner(parse, partOne, partTwo);
