const runner = require('../../utils/runner');

const parse = Number;

const sleep = (ms) => {
  const now = Date.now();
  while (Date.now() < now + ms) {
    Math.random();
  }
};

const nextElf = (elves, i) => {
  const idx = (j) => (j + elves.length) % elves.length;
  nextIdx = idx(i);
  next = elves[nextIdx];
  while (true) {
    if (next.n > 0) {
      return { elf: next, idx: nextIdx };
    }

    nextIdx = idx(nextIdx + 1);
    next = elves[nextIdx];
  }
};

const partOne = (n) => {
  const elves = Array.from({ length: n }).map((_, i) => ({ id: i + 1, n: 1 }));

  let i = 0;
  while (true) {
    // restart
    if (i === elves.length) i = 0;

    const elf = elves[i];
    // skip if no presents
    if (elf.n === 0) {
      i++;
      continue;
    }

    const { elf: next, idx: nextIdx } = nextElf(elves, i + 1);
    elves[i].n += next.n;
    elves[nextIdx].n = 0;

    if (elves[i].n === elves.length) {
      return elves[i].id;
    }

    i++;
  }
};

const partTwoOld = (n) => {
  let elves = Array.from({ length: n }).map((_, i) => ({ id: i + 1, n: 1 }));
  let idx = 0;
  while (elves.length > 1) {
    const elf = elves[idx];
    const nextIdx = idx + Math.floor(elves.length / 2);
    const { elf: next } = nextElf(elves, nextIdx);
    elves[idx].n += next.n;
    elves = elves.filter((elf) => elf.id !== next.id);
    idx = elves.findIndex((x) => x.id === elf.id);
    idx = (idx + 1 + elves.length) % elves.length;
  }

  return elves[0]?.id;
};

const partTwoOld2 = (n) => {
  const elves = Array.from({ length: n })
    .map((_, i) => ({ id: i + 1, n: 1 }))
    .reduce((acc, elf, i) => ({ ...acc, [i]: elf }), {});

  let count = n;
  let idx = 0;

  while (count > 1) {
    const targetIdx = idx + Math.floor(count / 2);
    const target = elves[targetIdx];

    console.log({
      elves,
      idx,
      count,
      targetIdx,
      target,
    });

    elves[idx].n += target.n;
    delete elves[targetIdx];
    count--;

    idx = (idx + 1 + count) % count;
  }

  return elves;
};

const partTwoSlow = (n) => {
  let elves = [];
  for (let i = 1; i <= n; i++) {
    elves.push(i);
  }

  let idx = 0;
  let p = 0;
  let now = Date.now();
  while (elves.length > 1) {
    const percent = Math.floor((100 * (n - elves.length)) / n);
    if (percent > p) {
      p = percent;
      const n = Date.now();
      const diff = n - now;
      now = n;
      //console.log(`${p}%, ${diff}ms`);
    }

    const target = (idx + Math.floor(elves.length / 2)) % elves.length;
    elves.splice(target, 1);
    if (target > idx) {
      idx = (idx + 1) % elves.length;
    }
  }

  return elves[0];
};

// 158236 too low
//

function getSafePosition(n) {
  // Find value of L for the equation
  let valueOfL = n - highestOneBit(n);
  return 2 * valueOfL + 1;
}

function highestOneBit(n) {
  return 1 << Math.floor(Math.log2(n));
}

const partTwo = (n) => getSafePosition(n);
//runner(parse, partOne, partTwo);

const compareResults =
  (...fs) =>
  (n) => {
    const header = [`n `, ...fs.map((f) => f.name + '(n)')];
    const headerStr = header.join(' | ');
    const topBorder = '-'.repeat(headerStr.length);
    console.log(headerStr + '\n' + topBorder);

    for (let i = 1; i <= n; i++) {
      const results = [i.toString().padStart(2, ' ')];
      for (let j = 0; j < fs.length; j++) {
        const f = fs[j];
        const y = f(i);
        const headerLength = header[j + 1].length;
        results.push(y.toString().padStart(headerLength, ' '));
      }
      console.log(results.join(' | '));
    }
  };

compareResults(getSafePosition, partTwo, partTwoSlow)(81);
