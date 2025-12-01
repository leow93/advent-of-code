process.env.TZ = 'UTC';
const runner = require('../../utils/runner');
const parseType = (desc) => {
  switch (desc) {
    case 'falls asleep':
      return 'falls_asleep';
    case 'wakes up':
      return 'wakes_up';
    default:
      return 'begins_shift';
  }
};
const parse = (input) => {
  const lines = input.split('\n');
  const result = [];
  for (const line of lines) {
    if (!line || line === '\n') continue;

    const reg =
      /^\[(\d{4}-\d{2}-\d{2} \d\d:\d\d)\] (falls asleep|wakes up|Guard #(\d+) begins shift)$/gi;
    const [_, time, desc, id] = reg.exec(line);

    result.push({
      time: new Date(time),
      type: parseType(desc),
      id,
    });
  }

  const sorted = result.sort((a, b) => a.time.valueOf() - b.time.valueOf());
  let guard = null;
  for (let i = 0; i < sorted.length; i++) {
    const item = sorted[i];
    if (item.type === 'begins_shift') {
      guard = item.id;
      continue;
    }

    if (!guard) throw new Error('expected guard to be defined');
    sorted[i].id = guard;
  }

  return sorted;
};

const minuteRange = (fellAsleep, wokeUp) => {
  const result = [];
  let temp = new Date(fellAsleep);
  while (temp < wokeUp) {
    result.push(temp.getMinutes());
    // add a minute
    temp = new Date(temp.getTime() + 60000);
  }
  return result;
};

const reducer = (state, action) => {
  switch (action.type) {
    case 'begins_shift': {
      return {
        ...state,
        [action.id]: state[action.id] ?? {},
        current: { id: action.id, started_shift: action.time },
      };
    }
    case 'falls_asleep': {
      return {
        ...state,
        [action.id]: state[action.id] ?? {},
        current: { ...state.current, fell_asleep: action.time },
      };
    }
    case 'wakes_up': {
      const date = action.time.toISOString().split('T')[0];
      return {
        ...state,
        [action.id]: {
          ...(state[action.id] ?? {}),
          [date]: [
            ...(state[action.id]?.[date] ?? []),
            minuteRange(state.current.fell_asleep, action.time),
          ],
        },
        current: {
          id: state.current.id,
          started_shift: state.current.started_shift,
        },
      };
    }
  }
};

const partOne = (input) => {
  const state = input.reduce(reducer, {});
  const ids = input.map((x) => x.id);

  let mostAsleepGuard;
  let max = 0;
  for (const id of ids) {
    const guard = state[id];
    const minutesAsleep = Object.values(guard).reduce(
      (count, day) => count + day.flat().length,
      0
    );
    if (minutesAsleep > max) {
      max = minutesAsleep;
      mostAsleepGuard = id;
    }
  }

  const minuteCounts = {};
  for (const day of Object.values(state[mostAsleepGuard])) {
    for (const minute of day.flat()) {
      minuteCounts[minute] = (minuteCounts[minute] ?? 0) + 1;
    }
  }

  max = 0;
  let result = -1;
  for (const [minute, count] of Object.entries(minuteCounts)) {
    if (count > max) {
      max = count;
      result = Number(minute);
    }
  }

  return result * Number(mostAsleepGuard);
};
const partTwo = (input) => {
  const state = input.reduce(reducer, {});
  const ids = Array.from(new Set(input.map((x) => x.id)));
  const guardMinuteFrequency = [];
  for (const id of ids) {
    const guard = state[id];
    const counts = {};
    for (const minutes of Object.values(guard).flat()) {
      for (const minute of minutes) {
        counts[minute] = (counts[minute] ?? 0) + 1;
      }
    }

    guardMinuteFrequency.push({ id, counts });
  }

  let guard;
  let minute;
  let max = 0;
  for (const g of guardMinuteFrequency) {
    for (const [m, count] of Object.entries(g.counts)) {
      if (count > max) {
        max = count;
        minute = Number(m);
        guard = g.id;
      }
    }
  }

  return guard * minute;
};

runner(parse, partOne, partTwo);
