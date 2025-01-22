const runner = require('../../utils/runner');

const parse = (input) => {
  const map = {};
  for (const line of input.split('\n')) {
    if (!line || line === '\n') continue;
    const [id, others] = line.split(' <-> ');
    map[id] = map[id] ?? [];

    for (const x of others.split(', ')) {
      map[id].push(x);
    }
  }
  return map;
};

const getGroup = (data, id) => {
  const stack = [id];
  const visited = new Set();
  while (stack.length) {
    const curr = stack.pop();
    if (!visited.has(curr)) {
      visited.add(curr);
      for (const x of data[curr]) {
        stack.push(x);
      }
    }
  }
  return visited;
};

const partOne = (data) => getGroup(data, '0').size;

const partTwo = (data) => {
  let sum = 0;
  const seen = new Set();
  for (const id of Object.keys(data)) {
    if (seen.has(id)) continue;
    const group = getGroup(data, id);
    for (const item of group.values()) {
      seen.add(item);
    }
    sum++;
  }
  return sum;
};

runner(parse, partOne, partTwo);
