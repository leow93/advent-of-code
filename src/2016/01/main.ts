import { readFileSync } from 'fs';
import path from 'path';

enum Direction {
  N,
  S,
  E,
  W,
}

type Instruction = { type: 'L'; value: number } | { type: 'R'; value: number };

const input = readFileSync(path.join(__dirname, './input.txt')).toString();
const instructions: Instruction[] = input.split(', ').map((line) => ({
  type: line.startsWith('R') ? 'R' : 'L',
  value: Number(line.substring(1)),
}));

const distance = ([x, y]: [number, number]) => Math.abs(x) + Math.abs(y);

type State = [[number, number], Direction];

const initialState: State = [[0, 0], Direction.N];

const moveDistanceInDirection = (
  [[x, y], direction]: State,
  distance: number
): State => {
  switch (direction) {
    case Direction.N:
      return [[x, y + distance], direction];
    case Direction.S:
      return [[x, y - distance], direction];
    case Direction.E:
      return [[x + distance, y], direction];
    case Direction.W:
      return [[x - distance, y], direction];
  }
};

const evolve = (state: State, instruction: Instruction): State => {
  const [[x, y], direction] = state;
  switch (direction) {
    case Direction.N:
      switch (instruction.type) {
        case 'L':
          return moveDistanceInDirection(
            [[x, y], Direction.W],
            instruction.value
          );
        case 'R':
          return moveDistanceInDirection(
            [[x, y], Direction.E],
            instruction.value
          );
        default:
          return state;
      }
    case Direction.S:
      switch (instruction.type) {
        case 'L':
          return moveDistanceInDirection(
            [[x, y], Direction.E],
            instruction.value
          );
        case 'R':
          return moveDistanceInDirection(
            [[x, y], Direction.W],
            instruction.value
          );
        default:
          return state;
      }
    case Direction.W:
      switch (instruction.type) {
        case 'L':
          return moveDistanceInDirection(
            [[x, y], Direction.S],
            instruction.value
          );
        case 'R':
          return moveDistanceInDirection(
            [[x, y], Direction.N],
            instruction.value
          );
        default:
          return state;
      }
    case Direction.E:
      switch (instruction.type) {
        case 'L':
          return moveDistanceInDirection(
            [[x, y], Direction.N],
            instruction.value
          );
        case 'R':
          return moveDistanceInDirection(
            [[x, y], Direction.S],
            instruction.value
          );
        default:
          return state;
      }
  }
};

const partOne = distance(instructions.reduce(evolve, initialState)[0]);

console.log('Part I ', partOne);

const keys = {
  toString: ([x, y]: [number, number]) => `${x}:${y}`,
  fromString: (s: string) => {
    const [x, y] = s.split(':');
    return [Number(x), Number(y)] as const;
  },
};

const partTwoCommands = ([_, direction]: State, instruction: Instruction) => {
  switch (direction) {
    case Direction.N:
      switch (instruction.type) {
        case 'L':
          return Array.from({ length: instruction.value }).map(
            () => Direction.W
          );
        case 'R':
          return Array.from({ length: instruction.value }).map(
            () => Direction.E
          );
        default:
          return [];
      }
    case Direction.S:
      switch (instruction.type) {
        case 'L':
          return Array.from({ length: instruction.value }).map(
            () => Direction.E
          );
        case 'R':
          return Array.from({ length: instruction.value }).map(
            () => Direction.W
          );
        default:
          return [];
      }
    case Direction.W:
      switch (instruction.type) {
        case 'L':
          return Array.from({ length: instruction.value }).map(
            () => Direction.S
          );
        case 'R':
          return Array.from({ length: instruction.value }).map(
            () => Direction.N
          );
        default:
          return [];
      }
    case Direction.E:
      switch (instruction.type) {
        case 'L':
          return Array.from({ length: instruction.value }).map(
            () => Direction.N
          );
        case 'R':
          return Array.from({ length: instruction.value }).map(
            () => Direction.S
          );
        default:
          return [];
      }
  }
};

const partTwo = () => {
  function inner(
    visited: Map<string, number>,
    [[x, y], d]: State,
    commands: Direction[],
    idx: number
  ): [Map<string, number>, State, [number, number] | null] {
    const direction = commands[idx];
    if (direction == undefined) {
      return [visited, [[x, y], d], null];
    }
    const nextState = moveDistanceInDirection([[x, y], direction], 1);
    const coord = nextState[0];
    const key = keys.toString(coord);
    const nextVisited = visited.set(key, (visited.get(key) ?? 0) + 1);
    const count = nextVisited.get(key) ?? 0;
    if (count === 2) {
      return [nextVisited, nextState, coord];
    }
    return inner(nextVisited, nextState, commands, idx + 1);
  }

  function loop(
    visited: Map<string, number>,
    state: State,
    idx: number
  ): [number, number] | null {
    const instruction = instructions[idx];
    if (instruction == undefined) return null;

    const commands = partTwoCommands(state, instruction);

    const [v, s, coord] = inner(visited, state, commands, 0);
    if (coord == null) {
      return loop(v, s, idx + 1);
    }
    return coord;
  }

  return loop(new Map(), initialState, 0);
};
console.log('Part II', distance(partTwo() ?? [0, 0]));
