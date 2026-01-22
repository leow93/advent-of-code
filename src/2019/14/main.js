const runner = require('../../utils/runner');

const parseChemical = (x) => {
  const [q, name] = x.split(' ');
  return {
    quantity: Number(q),
    name,
  };
};

const parse = (data) => {
  const result = [];
  for (const line of data.split('\n')) {
    if (!line) continue;
    const [input, out] = line.split(' => ');
    result.push({
      out: parseChemical(out),
      in: input.split(', ').map(parseChemical),
    });
  }
  return result;
};

const oreForFuel = (data, fuel) => {
  const reactions = {};
  for (const r of data) {
    reactions[r.out.name] = r;
  }
  const needs = {};
  needs['FUEL'] = fuel;
  const leftovers = {};
  const q = ['FUEL'];

  while (q.length > 0) {
    const curr = q.pop();
    const need = needs[curr] ?? 0;
    const leftover = leftovers[curr] ?? 0;
    const used = Math.min(need, leftover);
    needs[curr] = need - used;
    leftovers[curr] = leftover - used;
    if (needs[curr] === 0) continue;

    const r = reactions[curr];
    if (!r) continue;

    const runCount = Math.ceil(needs[curr] / r.out.quantity);
    const produced = runCount * r.out.quantity;
    leftovers[curr] = (leftovers[curr] ?? 0) + produced - needs[curr];

    for (const x of r.in) {
      needs[x.name] = (needs[x.name] ?? 0) + runCount * x.quantity;
      q.push(x.name);
    }

    needs[curr] = 0;
  }

  return needs['ORE'];
};

const partOne = (data) => oreForFuel(data, 1);

const partTwo = (data) => {
  const LIMIT = 1_000_000_000_000;
  let lo = 1;
  let hi = 1;

  while (true) {
    const ore = oreForFuel(data, hi);
    if (ore < LIMIT) {
      hi *= 2;
      continue;
    }
    break;
  }

  while (lo <= hi) {
    const mid = Math.floor((lo + hi) / 2);
    const ore = oreForFuel(data, mid);

    if (ore <= LIMIT) {
      lo = mid + 1;
    } else {
      hi = mid - 1;
    }
  }
  return hi;
};

runner(parse, partOne, partTwo);
