const runner = require('../../utils/runner');

const parse = (input) => {
  const lines = input.split('\n');
  const layers = [];
  for (const line of lines) {
    if (!line || line === '\n') continue;
    const [i, range] = line.split(': ').map(Number);
    layers[i] = Array.from({ length: range }).fill(null);
  }

  for (let i = 0; i < layers.length; i++) {
    const l = layers[i];
    if (!l) {
      layers[i] = [];
    }
  }

  return {
    layers,
  };
};

function findNthTermSymmetric(n, peak) {
  const period = 2 * peak; // Length of the repeating cycle
  const mod = n % period; // Position within the cycle
  return mod <= peak ? mod : 2 * peak - mod;
}

const partOne = (data) => {
  const { layers } = data;
  const caught = [];

  for (let i = 0; i < layers.length; i++) {
    const layer = layers[i];
    if (!layer.length) continue;
    const value = findNthTermSymmetric(i, layer.length - 1);
    if (value === 0) {
      caught.push({ depth: i, range: layer.length });
    }
  }

  return caught.reduce((sum, x) => sum + x.depth * x.range, 0);
};
const partTwo = (data) => {
  const { layers } = data;
  let delay = 1;
  while (true) {
    let caught = false;
    for (let i = 0; i < layers.length; i++) {
      const layer = layers[i];
      if (!layer.length) continue;
      const value = findNthTermSymmetric(delay + i, layer.length - 1);
      if (value === 0) {
        caught = true;
        break;
      }
    }

    if (!caught) return delay;

    delay++;
  }
};

runner(parse, partOne, partTwo);
