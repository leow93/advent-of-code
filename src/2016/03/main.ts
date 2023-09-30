import { readFileSync } from 'fs';
import path from 'path';

const input = readFileSync(path.join(__dirname, './input.txt'))
  .toString()
  .split('\n');

const toTriple = <T>(array: T[]) => {
  if (array.length === 3) {
    return [array[0], array[1], array[3]] as const;
  }

  return null;
};

const keepMap = <T, R>(array: T[], fn: (x: T) => R | null | undefined) => {
  const result: R[] = [];
  for (const x of array) {
    const y = fn(x);
    if (y != null) {
      result.push(y);
    }
  }
  return result;
};
console.log(input);
const triangles = keepMap(input, (line) =>
  toTriple(line.trim().split(/\s+/g).map(Number))
);

console.log(triangles);
