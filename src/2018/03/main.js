const runner = require('../../utils/runner');
const parse = (input) =>
  input
    .split('\n')
    .filter((x) => x !== '' && x !== '\n')
    .map((line) => {
      const reg = /^#(\d+) @ (\d+),(\d+): (\d+)x(\d+)$/;
      const exec = reg.exec(line);
      if (exec.length < 6) return null;
      const [_, id, x, y, width, height] = exec;

      return {
        id: Number(id),
        x: Number(x),
        y: Number(y),
        width: Number(width),
        height: Number(height),
      };
    });

const partOne = (data) => {
  const coords = new Map();
  for (const { id, x, y, width, height } of data) {
    for (let i = y; i < y + height; i++) {
      for (let j = x; j < x + width; j++) {
        const k = `${i},${j}`;
        if (!coords.has(k)) {
          coords.set(k, new Set());
        }
        coords.set(k, coords.get(k).add(id));
      }
    }
  }

  let count = 0;
  for (const ids of coords.values()) {
    if (ids.size > 1) count++;
  }
  return count;
};

const union = (as, bs) => {
  const result = new Set();
  for (let i = 0; i < as.length; i++) {
    for (let j = i + 1; j < as.length; j++) {
      const a = as[i];
      const b = bs[j];
      if (!b) break;

      if (a[0] === b[0] && a[1] === b[1]) result.add(`${a[0]},${a[1]}`);
    }
  }

  for (let i = 0; i < bs.length; i++) {
    for (let j = i + 1; j < as.length; j++) {
      const b = bs[i];
      const a = as[j];
      if (!a) break;

      if (a[0] === b[0] && a[1] === b[1]) result.add(`${a[0]},${a[1]}`);
    }
  }
  return Array.from(result).map((x) => x.split(',').map(Number));
};

const partTwo = (data) => {
  const claims = data.map(({ id, x, y, width, height }) => {
    const coords = [];
    for (let i = y; i < y + height; i++) {
      for (let j = x; j < x + width; j++) {
        coords.push([i, j]);
      }
    }

    return {
      id,
      coords,
    };
  });
  return claims.find((claim) => {
    const sharesACoord = claim.coords.some(([x0, y0]) => {
      return claims.some((other) => {
        return (
          other.id !== claim.id &&
          other.coords.some(([x1, y1]) => x0 === x1 && y0 === y1)
        );
      });
    });

    return !sharesACoord;
  })?.id;
};

runner(parse, partOne, partTwo);
