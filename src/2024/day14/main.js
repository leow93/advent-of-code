const { createCanvas } = require('canvas');
const fs = require('fs');
const runner = require('../../utils/runner');
const fm = require('../../utils/filterMap');
const parse = (input) =>
  fm(input.split('\n'), (line) => {
    if (!line || line === '\n') return null;

    const [position, velocity] = line.split(' ');

    return {
      position: position.replace('p=', '').split(',').map(Number),
      velocity: velocity.replace('v=', '').split(',').map(Number),
    };
  });

// x measured from left
// y measured from top

const countQuadrants = (height, width, positions) => {
  const wMid = (width - 1) / 2;
  const hMid = (height - 1) / 2;
  let a = 0;
  let b = 0;
  let c = 0;
  let d = 0;

  for (const [position, bots] of Object.entries(positions)) {
    const ps = position.split(',').map(Number);
    const [x, y] = ps;

    if (x < wMid && y < hMid) {
      a += bots.length;
    } else if (x < wMid && y > hMid) {
      c += bots.length;
    } else if (x > wMid && y < hMid) {
      b += bots.length;
    } else if (x > wMid && y > hMid) {
      d += bots.length;
    }
  }

  return [a, b, c, d];
};

const simulate = (positions, getNextPosition, n) => {
  const result = {};
  for (const [position, robots] of Object.entries(positions)) {
    for (const v of robots) {
      const p = getNextPosition(position, v, n);

      result[p] = result[p] ?? [];
      result[p].push(v);
    }
  }
  return result;
};

const get = (length, i) => ((i % length) + length) % length;
const gridHeight = 103;
const gridWidth = 101;

const nextPosition = (position, velocity, n) => {
  const ps = position.split(',').map(Number);
  const [x, y] = ps;
  const [vx, vy] = velocity;

  const x1 = get(gridWidth, x + n * vx);

  const y1 = get(gridHeight, y + n * vy);
  return `${x1},${y1}`;
};

const partOne = (robots) => {
  const positions = {};

  for (const bot of robots) {
    const key = [bot.position[0], bot.position[1]];
    positions[key] = positions[key] ?? [];
    positions[key].push(bot.velocity);
  }

  const resultPositions = simulate(positions, nextPosition, 100);
  return countQuadrants(gridHeight, gridWidth, resultPositions).reduce(
    (acc, x) => acc * x,
    1
  );
};

const writePicture = (positions, i) => {
  const width = gridWidth;
  const height = gridHeight;
  const canvas = createCanvas(width, height);

  const ctx = canvas.getContext('2d');

  for (const position of Object.keys(positions)) {
    const ps = position.split(',').map(Number);
    const [x, y] = ps;
    ctx.fillRect(x, y, 1, 1);
  }

  const out = fs.createWriteStream(__dirname + `/output_${i}.png`);
  const stream = canvas.createPNGStream();

  stream.pipe(out);

  return new Promise((resolve, reject) => {
    out.on('finish', resolve);
    out.on('error', reject);
  });
};

const isTree = (n, positions) => {
  // guess that it's a tree if one quadrant is particularly concentrated
  return countQuadrants(gridHeight, gridWidth, positions).some(
    (q) => q > n / 2
  );
};

const partTwo = async (robots) => {
  const n = robots.length;
  let positions = {};

  for (const bot of robots) {
    const key = [bot.position[0], bot.position[1]];
    positions[key] = positions[key] ?? [];
    positions[key].push(bot.velocity);
  }

  for (let i = 1; i < 10_000; i++) {
    const ps = simulate(positions, nextPosition, i);

    if (isTree(n, ps)) {
      await writePicture(positions, i);
      return i;
    }
  }
};

runner(parse, partOne, partTwo);
