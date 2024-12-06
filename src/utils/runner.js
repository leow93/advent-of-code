const readFromStdin = require('./readFromStdin');

const each =
  (...fs) =>
  (...args) =>
    fs.map((f) => f(...args));

const runner = (parser, partOne, partTwo) =>
  readFromStdin().then(parser).then(each(partOne, partTwo)).then(console.log);

module.exports = runner;
