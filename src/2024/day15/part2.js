const part2 = require('./part2_internal');

module.exports = (input) => {
  const { instructions, ...state } = part2.parse(input);
  const result = instructions.reduce(part2.reducer, state);
  return part2.sumGPSCoords(result);
};
