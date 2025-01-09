const runner = require('../../utils/runner');

const toValidator = (line) => {
  const [field, _ranges] = line.split(': ');

  const ranges = _ranges
    .split(' or ')
    .map((range) => range.split('-').map(Number));

  return {
    field,
    validate: (x) => ranges.some((range) => x >= range[0] && x <= range[1]),
  };
};

const parse = (input) => {
  const [fields, myTicket, nearbyTickets] = input.split('\n\n');

  return {
    validators: fields.split('\n').map(toValidator),
    myTicket: myTicket
      .split('\n')
      .slice(1)
      .map((line) => line.split(',').map(Number))[0],

    nearbyTickets: nearbyTickets
      .split('\n')
      .slice(1)
      .map((line) => line.split(',').map(Number)),
  };
};

const partOne = (data) => {
  const { validators, nearbyTickets } = data;
  return nearbyTickets.reduce((sum, ticket) => {
    let invalidFieldSum = 0;

    for (const field of ticket) {
      if (!validators.some(({ validate }) => validate(field))) {
        invalidFieldSum += field;
      }
    }

    return sum + invalidFieldSum;
  }, 0);
};
const partTwoOld = (data) => {
  const { validators, myTicket, nearbyTickets } = data;
  const validTickets = nearbyTickets.filter((ticket) => {
    return ticket.every((field) => validators.some((v) => v.validate(field)));
  });

  const fields = Array.from({ length: validators.length }).map(() => []);
  for (const { field, validate } of validators) {
    const length = validTickets[0].length;
    for (let i = 0; i < length; i++) {
      let correct = true;
      for (const ticket of validTickets) {
        correct = correct && validate(ticket[i]);
      }

      if (!correct) continue;

      fields[i].push(field);

      // field is taken, continue
    }
  }

  const result = Array.from({ length: fields.length }).fill(null);
  for (let i = 0; i < fields.length; i++) {
    const field = fields[i];
    console.log({
      i,
      field,
    });

    if (field.length === 1) {
      // Only possible option
      result[i] = field[0];
      continue;
    } else {
      const available = field.filter((x) => !result.includes(x));
      console.log('available', available);
      if (available.length === 1) {
        result[i] = available[0];
      }
    }
  }

  console.log('result', result);

  const departureFields = result
    .map((field, idx) => (field.startsWith('departure') ? idx : null))
    .filter((x) => x !== null);

  console.log(departureFields);
};
const partTwo = (data) => {
  const { validators, myTicket, nearbyTickets } = data;
  const validTickets = nearbyTickets.filter((ticket) => {
    return ticket.every((field) => validators.some((v) => v.validate(field)));
  });
  const nFields = myTicket.length;

  const possibilities = {};
  for (let i = 0; i < nFields; i++) {
    for (const { field, validate } of validators) {
      // Every ticket's current field is valid
      if (validTickets.every((ticket) => validate(ticket[i]))) {
        possibilities[field] = possibilities[field] ?? [];
        possibilities[field].push(i);
      }
    }
  }

  const queue = Object.entries(possibilities);
  const fieldPositions = Array.from({ length: nFields }).fill(null);

  while (queue.length > 0) {
    const [field, positions] = queue.shift();
    if (positions.length === 1) {
      fieldPositions[positions[0]] = field;
      continue;
    }

    // there are multiple options
    // filter out the taken ones
    const newPositions = positions.filter(
      (idx) => fieldPositions[idx] === null
    );
    queue.push([field, newPositions]);
  }

  return myTicket.reduce((result, field, i) => {
    if (!fieldPositions[i].startsWith('departure')) return result;

    return result * field;
  }, 1);
};

runner(parse, partOne, partTwo);
