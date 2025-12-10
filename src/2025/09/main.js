const runner = require('../../utils/runner');

const parse = (x) =>
  x
    .split('\n')
    .filter(Boolean)
    .map((line) => line.split(',').map(Number));

const getRectangles = (coords) => {
  const result = [];
  for (let i = 0; i < coords.length; i++) {
    for (let j = i + 1; j < coords.length; j++) {
      result.push([coords[i], coords[j]]);
    }
  }
  return result;
};

const getBoundary = (coords) => {
  const result = [];
  for (let i = 0; i < coords.length; i++) {
    if (i === coords.length - 1) {
      result.push([coords[i], coords[0]]);
      continue;
    }
    result.push([coords[i], coords[i + 1]]);
  }
  return result;
};

function aabb(boundary, rect) {
  const [[x1, y1], [x2, y2]] = sortBoxCoords(rect);
  for (const segment of boundary) {
    const [[sx1, sy1], [sx2, sy2]] = sortBoxCoords(segment);
    if (x1 < sx2 && x2 > sx1 && y1 < sy2 && y2 > sy1) {
      return true;
    }
  }
  return false;
}
const sortBoxCoords = ([[x1, y1], [x2, y2]]) => {
  const nx1 = Math.min(x1, x2);
  const nx2 = Math.max(x1, x2);
  const ny1 = Math.min(y1, y2);
  const ny2 = Math.max(y1, y2);
  return [
    [nx1, ny1],
    [nx2, ny2],
  ];
};

const partOne = (coords) => {
  let max = 0;
  for (const [a, b] of getRectangles(coords)) {
    const h = 1 + Math.abs(a[0] - b[0]);
    const w = 1 + Math.abs(a[1] - b[1]);

    const area = h * w;
    if (area > max) {
      max = area;
    }
  }
  return max;
};

const partTwo = (coords) => {
  const rectangles = getRectangles(coords).map(([a, b]) => {
    const h = 1 + Math.abs(a[0] - b[0]);
    const w = 1 + Math.abs(a[1] - b[1]);

    const area = h * w;
    return { a, b, area };
  });
  rectangles.sort((a, b) => b.area - a.area);
  const edges = getBoundary(coords);
  for (const r of rectangles) {
    const { a, b, area } = r;
    if (!aabb(edges, [a, b])) {
      return area;
    }
  }
  throw new Error('not found');
};
runner(parse, partOne, partTwo);
