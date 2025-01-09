const runner = require('../../utils/runner');
const filterMap = require('../../utils/filterMap');

const parse = (input) => {
  const lines = input.split('\n');
  const time = Number(lines[0]);
  return {
    time,
    buses: lines[1].split(',').map((x) => (x === 'x' ? 'x' : Number(x))),
  };
};

const partOne = ({ time, buses }) => {
  const waitTimes = buses.map((b) => {
    if (b === 'x') return { bus: b, waitTime: Infinity };
    if (time % b === 0) return { bus: b, waitTime: 0 };

    let i = time + 1;
    while (i < 2 * time) {
      if (i % b === 0) {
        return { bus: b, waitTime: i - time };
      }
      i++;
    }
    throw new Error('not found');
  });

  const earliest = waitTimes.sort((a, b) => a.waitTime - b.waitTime)[0];
  return earliest.waitTime * earliest.bus;
};

function gcd(a, b) {
  a = Math.abs(a);
  b = Math.abs(b);

  if (b > a) {
    var temp = a;
    a = b;
    b = temp;
  }

  while (true) {
    a %= b;
    if (a === 0) {
      return b;
    }
    b %= a;
    if (b === 0) {
      return a;
    }
  }
}
function lcm(a, b) {
  if (b === 0) return 0;
  return (a * b) / gcd(a, b);
}

const partTwo = ({ buses }) => {
  const filteredBuses = buses
    .map((id, i) => ({ id, i }))
    .filter((obj) => obj.id !== 'x');

  let first_bus = filteredBuses.shift();

  // Also initialize the first period to this same ID value, that is, the first bus's period.
  let period = first_bus.id;
  let timestamp = first_bus.id;

  // Loop through the remaining buses
  for (let { id, i } of filteredBuses) {
    /**
     * While the current timestamp plus its offset does not evenly divide the current ID,
     * increment the timestamp by our period, because we _have_ to keep the alignment
     * of whatever we have locked in so far.
     */
    while ((timestamp + i) % id !== 0) {
      timestamp += period;
    }

    /**
     * As soon as we have an timestamp where things are aligned, set the period
     * equal to the least common multiple between the current period
     * and the current ID so that our previous work stays aligned with each iteration.
     *
     * @note Looking at our input, all the numbers are prime, so the LCM will
     *       always be `period * id`, but this makes it a bit more general.
     */
    period = lcm(period, id);
  }

  return timestamp;
};

runner(parse, partOne, partTwo);
