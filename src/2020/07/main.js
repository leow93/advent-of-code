const runner = require('../../utils/runner');
const filterMap = require('../../utils/filterMap');

const buildMap = (bags) => {
  const map = new Map();
  for (const bag of bags) {
    map.set(bag.colour, bag.children);
  }
  return map;
};

const parse = (input) => {
  const lines = input.split('\n');
  return filterMap(lines, (line) => {
    if (!line) return null;
    const [colour, rest] = line.split(' bags contain ');
    if (rest === 'no other bags.' || rest === '' || !rest) {
      return {
        colour,
        children: [],
      };
    }
    const children = rest.split(', ').flatMap((description) => {
      const re = /^(\d+) (.+) bags?.?$/i;
      const result = re.exec(description);
      const n = Number(result[1]);
      return Array.from({ length: n }).map(() => ({
        colour: result[2],
      }));
    });

    return {
      colour,
      children,
    };
  });
};

const dfs = (bag, map, search) => {
  const visited = new Set();
  const queue = [bag.colour];

  while (queue.length) {
    const colour = queue.shift();
    if (visited.has(colour)) {
      continue;
    }
    visited.add(colour);
    const bag = map.get(colour);
    if (!bag) continue;
    for (const child of bag) {
      if (child.colour === search) {
        return true;
      }
      queue.unshift(child.colour);
    }
  }
  return false;
};

const countBags = (bags, map, search) => {
  let count = 0;
  for (const b of bags) {
    if (dfs(b, map, search)) {
      count++;
    }
  }
  return count;
};

const countBagsInside = (map, start) => {
  const queue = [start];
  let count = 0;

  while (queue.length) {
    const colour = queue.shift();
    if (colour !== start) {
      count++;
    }
    const children = map.get(colour);
    if (!children) continue;
    for (const child of children) {
      queue.push(child.colour);
    }
  }

  return count;
};

const partOne = (bags) => {
  const map = buildMap(bags);
  return countBags(bags, map, 'shiny gold');
};

const partTwo = (bags) => {
  const map = buildMap(bags);
  return countBagsInside(map, 'shiny gold');
};

runner(parse, partOne, partTwo);
