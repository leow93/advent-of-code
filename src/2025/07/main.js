const runner = require('../../utils/runner');

const parse = (x) => {
  const lines = x.split('\n').filter(Boolean);
  const grid = [];
  let start;
  for (let i = 0; i < lines.length; i++) {
    const line = lines[i];
    const row = [];
    for (let j = 0; j < line.length; j++) {
      const x = line[j];
      if (x === 'S') start = [i, j];
      row.push(x);
    }
    grid.push(row);
  }
  return {
    grid,
    start,
  };
};
const run = (data) => {
  const { start, grid } = data;
  const beams = grid.map((row, i) => {
    return row.map((c, j) => {
      if (i === start[0] && j === start[1]) return { count: 1 };
      return null;
    });
  });

  let splitCount = 0;

  const addBeam = (i, j, count) => {
    if (i < 0 || i >= beams.length) return;
    if (j < 0 || j >= beams[0].length) return;
    const curr = beams[i][j];
    if (!curr) {
      beams[i][j] = { count };
      return;
    }

    beams[i][j].count += count;
  };
  for (let i = 1; i < grid.length; i++) {
    const row = grid[i];

    for (let j = 0; j < row.length; j++) {
      const beam = beams[i - 1][j];
      if (!beam) continue;

      const cell = grid[i][j];
      if (cell === '.') {
        addBeam(i, j, beam.count);
        continue;
      }

      if (cell === '^') {
        splitCount++;
        addBeam(i, j + 1, beam.count);
        addBeam(i, j - 1, beam.count);
      }
    }
  }
  const last = beams[beams.length - 1];
  const pathCount = last.reduce((c, b) => c + (b?.count ?? 0), 0);
  return { splitCount, pathCount };
};

const partOne = (data) => {
  return run(data).splitCount;
};
const partTwo = async (data) => {
  return run(data).pathCount;
};

runner(parse, partOne, partTwo);
