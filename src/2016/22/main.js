const runner = require('../../utils/runner');

/**
 * @param {string} input
 */
const parse = (input) =>
  input
    .split('\n')
    .filter((x) => x && x !== '\n')
    .slice(2)
    .map((line) => {
      const [id, size, used, available, usage] = line.split(/\s+/);

      return {
        id,
        size: Number(size.split('T')[0]),
        used: Number(used.split('T')[0]),
        available: Number(available.split('T')[0]),
        usage: Number(usage.split('%')[0]),
      };
    });

const partOne = (nodes) => {
  const pairs = new Set();
  let count = 0;
  for (let i = 0; i < nodes.length; i++) {
    const a = nodes[i];
    if (a.used === 0) continue;
    for (let j = 0; j < nodes.length; j++) {
      const b = nodes[j];
      if (a.id === b.id || a.used > b.available) continue;

      // viable!
      count++;
    }
  }
  // 161 too low
  return count;
};
const partTwo = (nodes) => null;
runner(parse, partOne, partTwo);
