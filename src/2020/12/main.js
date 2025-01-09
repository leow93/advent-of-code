const runner = require('../../utils/runner');
const filterMap = require('../../utils/filterMap');

const move = (direction, distance) => ({ type: 'move', direction, distance });
const turn = (direction, degrees) => ({ type: 'turn', direction, degrees });

const parse = (input) =>
  filterMap(input.split('\n'), (x) => {
    const instruction = x[0];

    switch (instruction) {
      case 'N':
      case 'E':
      case 'S':
      case 'W':
      case 'F':
        return move(instruction, Number(x.slice(1)));
      case 'L':
      case 'R':
        return turn(instruction, Number(x.slice(1)));
      default:
        return null;
    }
  });

const handleMove = (state, action) => {
  switch (action.direction) {
    case 'N':
      return { ...state, y: state.y + action.distance };
    case 'E':
      return { ...state, x: state.x + action.distance };
    case 'S':
      return { ...state, y: state.y - action.distance };
    case 'W':
      return { ...state, x: state.x - action.distance };
    case 'F':
      return handleMove(state, move(state.direction, action.distance));
  }
};

const sin = (degrees) => {
  switch (degrees) {
    case 90:
      return 1;
    case 180:
      return 0;
    case 270:
      return -1;
    case 360:
      return 0;
    default:
      throw new Error('unsupported ' + degrees);
  }
};

const cos = (degrees) => {
  switch (degrees) {
    case 90:
      return 0;
    case 180:
      return -1;
    case 270:
      return 0;
    case 360:
      return 1;
    default:
      throw new Error('unsupported ' + degrees);
  }
};

const handleMoveWaypoint = (state, action) => {
  switch (action.direction) {
    case 'N':
      return {
        ...state,
        waypoint: {
          x: state.waypoint.x,
          y: state.waypoint.y + action.distance,
        },
      };
    case 'E':
      return {
        ...state,
        waypoint: {
          y: state.waypoint.y,
          x: state.waypoint.x + action.distance,
        },
      };
    case 'S':
      return {
        ...state,
        waypoint: {
          x: state.waypoint.x,
          y: state.waypoint.y - action.distance,
        },
      };
    case 'W':
      return {
        ...state,
        waypoint: {
          y: state.waypoint.y,
          x: state.waypoint.x - action.distance,
        },
      };
    case 'F': {
      const dx = state.waypoint.x;
      const dy = state.waypoint.y;
      let x = state.x;
      let y = state.y;
      for (let i = 0; i < action.distance; i++) {
        x += dx;
        y += dy;
      }
      return { ...state, x, y };
    }
  }
};

const handleRotateWaypoint = (state, action) => {
  const dx = state.waypoint.x;
  const dy = state.waypoint.y;

  const degrees = action.degrees;

  const directionFactor = action.direction === 'R' ? -1 : 1;

  const x = (dx * cos(degrees) - dy * sin(degrees)) * directionFactor;
  const y = (dx * sin(degrees) + dy * cos(degrees)) * directionFactor;

  return {
    ...state,
    waypoint: {
      x,
      y,
    },
  };
};

const rightTurns = ['N', 'E', 'S', 'W'];
const turnRight = (curr, amount) => {
  let idx = rightTurns.findIndex((x) => x === curr);
  if (idx < 0) return curr;

  let result = curr;
  let toTurn = amount;
  while (toTurn > 0) {
    idx = idx === 3 ? 0 : idx + 1;
    result = rightTurns[idx];
    toTurn -= 90;
  }
  return result;
};
const leftTurns = ['N', 'W', 'S', 'E'];
const turnLeft = (curr, amount) => {
  let idx = leftTurns.findIndex((x) => x === curr);
  if (idx < 0) return curr;

  let result = curr;
  let toTurn = amount;
  while (toTurn > 0) {
    idx = idx === 3 ? 0 : idx + 1;
    result = leftTurns[idx];
    toTurn -= 90;
  }
  return result;
};
const handleTurn = (state, action) => {
  const amount = action.degrees % 360;
  switch (action.direction) {
    case 'L':
      return { ...state, direction: turnLeft(state.direction, action.degrees) };
    case 'R':
      return { ...state, direction: turnRight(state.direction, amount) };
    default:
      return state;
  }
};

const reducer = (state, action) => {
  switch (action.type) {
    case 'move':
      return handleMove(state, action);
    case 'turn':
      return handleTurn(state, action);
    default:
      return state;
  }
};

const waypointReducer = (state, action) => {
  switch (action.type) {
    case 'move':
      return handleMoveWaypoint(state, action);
    case 'turn':
      return handleRotateWaypoint(state, action);
    default:
      return state;
  }
};

const partOne = (instructions) => {
  const initialState = { direction: 'E', x: 0, y: 0 };
  const state = instructions.reduce(reducer, initialState);
  return Math.abs(state.x) + Math.abs(state.y);
};
const partTwo = (instructions) => {
  const initialState = {
    direction: 'E',
    x: 0,
    y: 0,
    waypoint: { x: 10, y: 1 },
  };
  const state = instructions.reduce(waypointReducer, initialState);
  // 65827 too high
  return Math.abs(state.x) + Math.abs(state.y);
};

runner(parse, partOne, partTwo);
