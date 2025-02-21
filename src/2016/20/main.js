const runner = require('../../utils/runner');

const parse = (input) =>
  input
    .split('\n')
    .filter((x) => x && x !== '\n')
    .map((line) => {
      const [min, max] = line.split('-').map(Number);
      return [min, max];
    });

const partOne = (ranges) => {
  const sorted = ranges.slice().sort((a, b) => a[0] - b[0]);

  for (let i = 0; i < sorted.length; i++) {
    const [_, b] = sorted[i];

    const m = b + 1;

    const other = sorted.find(([c, d]) => c <= m && m <= d);
    if (!other) return m;
  }
};

const partTwo = (ranges) => {
  const sorted = ranges.slice().sort((a, b) => a[0] - b[0]);
  let count = 0;

  const MAX = 4294967295;

  let allowedCount = 0;
  let nextAllowedIP = 0;

  for (const [start, end] of sorted) {
    if (nextAllowedIP < start) {
      allowedCount += start - nextAllowedIP;
    }
    nextAllowedIP = Math.max(nextAllowedIP, end + 1);
  }

  // Count remaining allowed IPs after the last blocked range
  if (nextAllowedIP <= MAX) {
    allowedCount += MAX - nextAllowedIP + 1;
  }

  return allowedCount;
};

runner(parse, partOne, partTwo);
