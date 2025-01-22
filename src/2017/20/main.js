const runner = require('../../utils/runner');
const filterMap = require('../../utils/filterMap');

const parseCoordinate = (token, coord) => {
  return coord
    .replaceAll(new RegExp(`${token}=<`, 'g'), '')
    .replaceAll(/>/g, '')
    .split(',')
    .map(Number);
};

const parse = (input) => {
  let id = 0;
  return filterMap(input.split('\n'), (line) => {
    if (!line || line === '\n') return null;

    const [p, v, a] = line.split(', ');

    return {
      id: id++,
      position: parseCoordinate('p', p),
      velocity: parseCoordinate('v', v),
      acceleration: parseCoordinate('a', a),
    };
  });
};
const run = (state, collisions = false) => {
  const positions = new Map();
  const key = (position) => position.join(',');
  const next = state.map((particle) => {
    const velocity = particle.velocity.map(
      (v, i) => v + particle.acceleration[i]
    );
    const position = particle.position.map((p, i) => p + velocity[i]);
    const particles = positions.get(key(position)) ?? [];
    particles.push(particle.id);
    positions.set(key(position), particles);
    return {
      ...particle,
      velocity,
      position,
    };
  });

  if (!collisions) return next;

  return next.filter((x) => {
    if (positions.get(key(x.position))?.length > 1) return false;

    return true;
  });
};

const manhattan = ([x, y, z]) => Math.abs(x) + Math.abs(y) + Math.abs(z);
const partOne = (data) => {
  let state = data.slice();
  let closest;
  let n = 0;
  while (n < 1000) {
    state = run(state);
    const deltas = state
      .map((p) => ({
        id: p.id,
        distance: manhattan(p.position),
      }))
      .sort((a, b) => a.distance - b.distance);

    closest = deltas[0];
    n++;
  }
  return closest;
};
const partTwo = (data) => {
  let state = data.slice();
  let n = 0;
  let length = state.length;
  while (n < 1000) {
    state = run(state, true);
    if (state.length < length) {
      length = state.length;
    }
    n++;
  }
  return length;
};
runner(parse, partOne, partTwo);
