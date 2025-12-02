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
const partTwo = ({ map, imap, roots }) => {
  const availableWorkers = 5;
  const seen = new Set();
  const q = roots.slice().sort(alphabetical);
  let order = '';
  let work = [];

  const ready = (x) => {
    const dependents = imap[x];
    if (!dependents) return true;
    return dependents?.every((d) => seen.has(d)) ?? true;
  };

  const ttl = (id) => {
    return 61 + id.charCodeAt(0) - 'A'.charCodeAt(0);
  };

  const simulateSecond = () => {
    // check if any work can be removed
    work = work
      .map((w) => ({ id: w.id, ttl: w.ttl - 1 }))
      .filter((item) => {
        // it's done by this tick
        const dead = item.ttl === 0;
        if (dead) {
          order += item.id;
          seen.add(item.id);

          const next = map[item.id];
          for (const x of next ?? []) {
            if (ready(x)) {
              q.push(x);
            } else {
            }
          }
          q.sort(alphabetical);
        }
        return !dead;
      });

    // Try to schedule work if we have some availability
    while (work.length < availableWorkers && q.length) {
      const nextWork = q.shift();
      if (seen.has(nextWork)) return;

      if (!ready(nextWork)) {
        // not ready, put this item back to the start of the queue
        q.unshift(nextWork);
        return;
      }

      work.push({ id: nextWork, ttl: ttl(nextWork) });
    }
  };

  let seconds = 0;
  while (true) {
    simulateSecond();
    if (q.length || work.length) {
      seconds++;
    } else {
      break;
    }
  }

  return seconds;
};

runner(parse, partOne, partTwo);
