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

function findAllWinners(boards) {
  const winners = [];
  for (let i = 0; i < boards.length; i++) {
    if (winners.includes(i)) {
      continue;
    }
    const board = boards[i];
    if (
      board.some((row) => row.every((cell) => cell.drawn)) ||
      transpose(board).some((row) => row.every((cell) => cell.drawn))
    ) {
      winners.push(i);
    }
  }
  return winners;
}

function calculateScore(board, numberDrawn) {
  return (
    board.reduce(
      (total, row) =>
        row.reduce(
          (sum, cell) => (cell.drawn ? sum : sum + cell.number),
          total
        ),
      0
    ) * numberDrawn
  );
}

function partOne(filename) {
  const input = readInput(filename);
  const { drawnNumbers } = input;
  let state = input.boards;
  for (const number of drawnNumbers) {
    state = evolve(state, number);
    const winner = checkWinner(state);
    if (winner != null) {
      const score = calculateScore(state[winner], number);
      console.log('SCORE', score);
      break;
    }
  }
}

function partTwo(filename) {
  const input = readInput(filename);
  const { drawnNumbers } = input;
  let state = input.boards;
  const orderedWinners = [];
  for (const number of drawnNumbers) {
    state = evolve(state, number);
    for (const x of findAllWinners(state)) {
      if (!orderedWinners.includes(x)) {
        orderedWinners.push(x);
      }
    }

    if (orderedWinners.length === state.length) {
      console.log(
        'Last winner is board number:',
        orderedWinners[orderedWinners.length - 1] + 1
      );
      console.log('number called is', number);
      console.log(
        'score is',
        calculateScore(state[orderedWinners[orderedWinners.length - 1]], number)
      );
      break;
    }
  }
}

function main() {
  console.log('part I test: ');
  partOne('test.txt');
  console.log('part I');
  partOne('data.txt');

  console.log('part II test: ');
  partTwo('test.txt');
  console.log('part II');
  partTwo('data.txt');
}

main();
