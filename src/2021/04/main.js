const fs = require('fs');
const path = require('path');

const emptyBoard = () => [
  [
    { number: 0, drawn: false },
    { number: 0, drawn: false },
    { number: 0, drawn: false },
    { number: 0, drawn: false },
    { number: 0, drawn: false },
  ],
  [
    { number: 0, drawn: false },
    { number: 0, drawn: false },
    { number: 0, drawn: false },
    { number: 0, drawn: false },
    { number: 0, drawn: false },
  ],
  [
    { number: 0, drawn: false },
    { number: 0, drawn: false },
    { number: 0, drawn: false },
    { number: 0, drawn: false },
    { number: 0, drawn: false },
  ],
  [
    { number: 0, drawn: false },
    { number: 0, drawn: false },
    { number: 0, drawn: false },
    { number: 0, drawn: false },
    { number: 0, drawn: false },
  ],
  [
    { number: 0, drawn: false },
    { number: 0, drawn: false },
    { number: 0, drawn: false },
    { number: 0, drawn: false },
    { number: 0, drawn: false },
  ],
];

function readInput(filename) {
  const lines = fs
    .readFileSync(path.resolve(__dirname, filename))
    .toString()
    .split('\n');

  const drawnNumbers = lines[0].split(',').map(Number);
  let boardIdx = 0;
  let boardRowIdx = 0;
  const boards = lines.slice(2).reduce(
    (state, line) => {
      if (line === '') {
        boardIdx++;
        boardRowIdx = 0;
        return [...state, emptyBoard()];
      }
      const reg = /\s+/g;
      state[boardIdx][boardRowIdx] = line
        .split(reg)
        .map((x) => parseInt(x))
        .filter((x) => typeof x === 'number' && !isNaN(x))
        .map((x) => ({
          number: x,
          drawn: false,
        }));
      boardRowIdx++;
      return state;
    },
    [emptyBoard()]
  );

  return { drawnNumbers, boards };
}

function evolve(boards, number) {
  return boards.map((board) =>
    board.map((row) =>
      row.map((cell) => {
        if (cell.drawn) return cell;

        return {
          number: cell.number,
          drawn: number === cell.number,
        };
      })
    )
  );
}

function transpose(matrix) {
  return Array.from({ length: matrix[0].length }).map((_, i) =>
    Array.from({ length: matrix.length }).map((_, j) => matrix[j][i])
  );
}
function checkWinner(boards) {
  for (let i = 0; i < boards.length; i++) {
    const board = boards[i];

    if (board.some((row) => row.every((cell) => cell.drawn))) {
      return i;
    }

    const transposed = transpose(board);
    if (transposed.some((row) => row.every((cell) => cell.drawn))) {
      return i;
    }
  }
}

function partOne(filename) {
  const input = readInput(filename);
  const { drawnNumbers } = input;
  let state = input.boards;
  for (const number of drawnNumbers) {
    state = evolve(state, number);
    const winner = checkWinner(state);
    if (winner != null) {
      console.log('Winner', winner, number);

      const score =
        state[winner].reduce((total, row) => {
          return row.reduce(
            (sum, cell) => (cell.drawn ? sum : sum + cell.number),
            total
          );
        }, 0) * number;
      console.log('SCORE', score);
      break;
    }
  }
}

function main() {
  partOne('test.txt');
  partOne('data.txt');
}

main();
