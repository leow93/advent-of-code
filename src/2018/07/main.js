const runner = require('../../utils/runner');

const getRoots = (map) => {
  const keys = Object.keys(map);
  const values = Object.values(map);

  const roots = keys.filter((key) => {
    return values.every((v) => !v.includes(key));
  });

  if (!roots.length) throw new Error('root not found');
  return roots;
};

const parse = (input) => {
  const lines = input.split('\n').filter((x) => x !== '');
  const imap = {};
  const map = {};
  for (const line of lines) {
    const re =
      /^Step ([A-Z]) must be finished before step ([A-Z]) can begin\.$/;
    const matches = re.exec(line);
    if (!matches) continue;
    const dependent = matches[1];
    const step = matches[2];

    imap[step] = imap[step] ?? [];
    imap[step].push(dependent);

    map[dependent] = map[dependent] ?? [];
    map[dependent].push(step);
  }

  return {
    map: map,
    imap: imap,
    roots: getRoots(map),
  };
};
const sleep = (ms) => new Promise((r) => setTimeout(r, ms));
const logger =
  (enabled) =>
  (...args) => {
    if (!enabled) return;
    return console.log(...args);
  };

const alphabetical = (a, b) => a.localeCompare(b);

const partOne = async ({ map, imap, roots }) => {
  const seen = new Set();
  const q = roots.slice().sort(alphabetical);
  let order = '';

  const ready = (x) => {
    const dependents = imap[x];
    if (!dependents) return true;
    return dependents?.every((d) => seen.has(d)) ?? true;
  };

  while (q.length) {
    const curr = q.shift();
    if (seen.has(curr)) {
      continue;
    }
    if (!ready(curr)) {
      q.push(curr);
      continue;
    }

    seen.add(curr);
    order += curr;

    const next = map[curr];
    for (const x of next ?? []) {
      if (ready(x)) q.push(x);
    }
    q.sort(alphabetical);
  }

  return order;
};
const partTwo = (input) => null;

runner(parse, partOne, partTwo);
