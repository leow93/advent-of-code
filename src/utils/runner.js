const readFromStdin = require('./readFromStdin');

const withPerf = async (f) => {
  const start = performance.now();
  const result = await Promise.resolve().then(() => f());
  const end = performance.now();
  return { result, duration: end - start };
};
const runner = async (parser, partOne, partTwo) => {
  const input = await readFromStdin();
  const data = parser(input);
  const p1 = await withPerf(() => partOne(data, input));
  console.log('Part I:', p1.result);
  console.log(`(${p1.duration} ms)`);
  const p2 = await withPerf(() => partTwo(data, input));
  console.log('Part II:', p2.result);
  console.log(`(${p2.duration} ms)`);
  return [p1, p2];
};
module.exports = runner;
