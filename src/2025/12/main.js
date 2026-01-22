const runner = require('../../utils/runner');

const shapeArea = (area, row) => {
  let x = 0;
  for (const ch of row) {
    x += ch === '#' ? 1 : 0;
  }
  return area + x;
};

const parseRegions = (str) =>
  str.split('\n').map((line) => {
    const [dims, qs] = line.split(': ');

    return {
      area: Number(dims.slice(0, 2)) * Number(dims.slice(3)),
      quantities: qs.split(' ').map(Number),
    };
  });
const parse = (input) => {
  const sections = input.split('\n\n');
  const shapes = sections
    .slice(0, sections.length - 1)
    .map((str) => str.split('\n').slice(1).reduce(shapeArea, 0));

  return {
    shapes,
    regions: parseRegions(sections[sections.length - 1]),
  };
};

const partOne = ({ shapes, regions }) => {
  let count = 0;
  for (const { area, quantities } of regions) {
    let a = 0;
    for (let i = 0; i < quantities.length; i++) {
      const shapesArea = quantities[i] * shapes[i];
      a += shapesArea;
    }
    if (a <= area) count++;
  }
  return count;
};
runner(parse, partOne, () => null);
