const runner = require('../../utils/runner');

const parse = (data) => {
  const lines = data.split('\n');
  const result = [];

  for (const line of lines) {
    if (!line || line === '\n') continue;
    const regexp =
      /Disc #(\d+) has (\d+) positions; at time=(\d+), it is at position (\d+)\./g;

    const match = regexp.exec(line);

    result.push({
      positions: Number(match[2]),
      initialPosition: Number(match[4]),
    });
  }

  return result;
};

const position = (disc, time) => {
  const p = disc.initialPosition + time;
  return p % disc.positions;
};

const partOne = (discs) => {
  for (let t = 0; true; t++) {
    let time = t;
    if (
      discs.every((disc) => {
        time++;
        return position(disc, time) === 0;
      })
    ) {
      return t;
    }
  }
};

const partTwo = (discs) =>
  partOne([...discs, { initialPosition: 0, positions: 11 }]);

runner(parse, partOne, partTwo);
