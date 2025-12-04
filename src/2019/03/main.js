const runner = require('../../utils/runner');

const parse = (x) =>
  x
    .split('\n')
    .map((wire, i) =>
      wire === ''
        ? null
        : wire.split(',').map((instr) => {
            const dir = instr[0];
            return {
              id: i,
              dir,
              amount: Number(instr.slice(1)),
            };
          })
    )
    .filter((x) => x !== null);

const manhattanDistance = (coord) => {
  return Math.abs(coord[0]) + Math.abs(coord[1]);
};
const smallestManhattanDistance = (coords) => {
  let min = Infinity;
  for (const coord of coords) {
    const d = manhattanDistance(coord);
    if (d < min) {
      min = d;
    }
  }
  return min;
};

const getIntersections = (wires) => {
  const coords = new Map();

  const key = (coord) => `${coord[0]},${coord[1]}`;

  const addCoord = (coord, orientation, wire_id) => {
    coords.set(
      key(coord),
      (coords.get(key(coord)) ?? []).concat({
        coord,
        orientation,
        wire_id: wire_id,
      })
    );
  };

  const handleInstr = (instr, curr) => {
    const orientation =
      instr.dir === 'L' || instr.dir === 'R' ? 'horizontal' : 'vertical';

    let coord = curr;
    for (let i = 0; i < instr.amount; i++) {
      switch (instr.dir) {
        case 'U':
          coord = [coord[0], coord[1] + 1];
          addCoord(coord, orientation, instr.id);
          break;
        case 'D':
          coord = [coord[0], coord[1] - 1];
          addCoord(coord, orientation, instr.id);
          break;
        case 'L':
          coord = [coord[0] - 1, coord[1]];
          addCoord(coord, orientation, instr.id);
          break;
        case 'R':
          coord = [coord[0] + 1, coord[1]];
          addCoord(coord, orientation, instr.id);
          break;
        default:
          break;
      }
    }

    return coord;
  };

  for (const wire of wires) {
    let curr = [0, 0];
    for (const instr of wire) {
      curr = handleInstr(instr, curr);
    }
  }

  const intersections = [];
  for (const [_, v] of coords.entries()) {
    if (v.length < 2) continue;
    intersections.push(v[0].coord);
  }

  return intersections;
};
const partOne = (wires) => {
  const intersections = getIntersections(wires);
  return smallestManhattanDistance(intersections);
};

const partTwo = (wires) => {
  const intersections = getIntersections(wires);
  let min = Infinity;

  for (const int of intersections) {
    let totalSteps = 0;
    for (const wire of wires) {
      let i = 0,
        j = 0;

      let steps = 0;
      let found = false;
      for (const instr of wire) {
        for (let k = 0; k < instr.amount; k++) {
          if (instr.dir === 'L') i--;
          if (instr.dir === 'R') i++;
          if (instr.dir === 'U') j++;
          if (instr.dir === 'D') j--;
          steps++;
          if (i === int[0] && j === int[1]) {
            found = true;
            break;
          }
        }

        if (found) break;
      }

      totalSteps += steps;
    }

    if (totalSteps < min) min = totalSteps;
  }

  return min;
};

runner(parse, partOne, partTwo);
