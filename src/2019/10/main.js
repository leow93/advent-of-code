const runner = require('../../utils/runner.js');

const parse = (x) => x.split('\n').map((line) => line.split(''));
const one = (grid) => {};
const two = (x) => x;
runner(parse, one, two);
