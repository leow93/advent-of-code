const readFromStdin = require('./readFromStdin');

const each =
  (...fs) =>
  (...args) =>
    Promise.resolve().then(() => Promise.all(fs.map((f) => f(...args))));

const tap = (f) => (x) => {
  f(x);
  return x;
};

const runner = (parser, partOne, partTwo) =>
  readFromStdin()
    .then(parser)
    .then(each(partOne, partTwo))
    .then(tap(console.log));

module.exports = runner;
