const runner = require('../../utils/runner');

const parse = (input) => {
  const specs = input.split('\n');
  return specs.map((spec) => {
    const rowspec = spec.slice(0, 7);
    const seatspec = spec.slice(7);

    return decode(rowspec, seatspec);
  });
};

const seatId = (row, col) => row * 8 + col;
const decodeRow = (r) => {
  const search = (lo, hi, idx) => {
    const ch = r[idx];
    if (hi - lo === 1) {
      return ch === 'F' ? lo : hi;
    }

    if (ch === 'F') {
      return search(lo, hi - Math.floor((hi + 1 - lo) / 2), idx + 1);
    }

    if (ch === 'B') {
      return search(lo + Math.ceil((hi + 1 - lo) / 2), hi, idx + 1);
    }

    throw new Error('unexpected ch: ' + ch);
  };

  return search(0, 127, 0);
};

const decodeSeat = (s) => {
  const search = (lo, hi, idx) => {
    const ch = s[idx];
    if (hi - lo === 1) {
      return ch === 'R' ? hi : lo;
    }

    if (ch === 'L') {
      return search(lo, hi - Math.floor((hi + 1 - lo) / 2), idx + 1);
    }

    if (ch === 'R') {
      return search(lo + Math.ceil((hi + 1 - lo) / 2), hi, idx + 1);
    }

    throw new Error('unexpected ch: ' + ch);
  };

  return search(0, 7, 0);
};
const decode = (rowspec, seatspec) => {
  const row = decodeRow(rowspec);
  const seat = decodeSeat(seatspec);
  return seatId(row, seat);
};

const partOne = (seatIds) => Math.max(...seatIds);
const partTwo = (seatIds) => {
  const seats = seatIds.sort((a, b) => {
    if (a > b) {
      return 1;
    }
    if (b > a) {
      return -1;
    }
    return 0;
  });

  for (let i = 0; i < seats.length - 1; i++) {
    const seat = seats[i];
    const next = seats[i + 1];
    if (next - seat !== 1) {
      return seat + 1;
    }
  }
  return null;
};

runner(parse, partOne, partTwo);
