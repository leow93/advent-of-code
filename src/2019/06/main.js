const runner = require('../../utils/runner');

const parse = (x) => {
  const lines = x.split('\n');
  const satellites = new Map();
  for (const line of lines) {
    const [host, satellite] = line.split(')');
    satellites.set(satellite, (satellites.get(satellite) ?? []).concat([host]));
  }
  return { satellites };
};

const getOrbits = (satellites, s) => {
  const hosts = satellites.get(s);
  if (!hosts) return [];
  const result = [];
  const queue = hosts.slice();
  while (queue.length) {
    const curr = queue.shift();
    result.push(curr);
    const hs = satellites.get(curr);
    if (!hs) continue;
    queue.push(...hs);
  }

  return result;
};

const countAllOrbits = (satellites) => {
  let count = 0;
  const queue = [];

  for (const s of satellites.keys()) {
    queue.push(s);
  }

  while (queue.length) {
    const curr = queue.shift();
    const hosts = satellites.get(curr);
    if (!hosts) continue;
    count += hosts.length;
    queue.push(...hosts);
  }
  return count;
};

const partOne = (data) => {
  const satellites = data.satellites;
  return countAllOrbits(satellites);
};
const partTwo = ({ satellites }) => {
  const me = getOrbits(satellites, 'YOU');
  const santas = getOrbits(satellites, 'SAN');

  for (let i = 0; i < me.length; i++) {
    for (let j = 0; j < santas.length; j++) {
      if (me[i] === santas[j]) return i + j;
    }
  }

  throw new Error('not found');
};

runner(parse, partOne, partTwo);
