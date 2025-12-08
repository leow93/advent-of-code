const runner = require('../../utils/runner');

const parse = (x) =>
  x
    .split('\n')
    .filter(Boolean)
    .map((line) => ({ id: line, coords: line.split(',').map(Number) }));

const square = (x) => x * x;

const distance = ([x1, y1, z1], [x2, y2, z2]) => {
  const dx2 = square(Math.abs(x2 - x1));
  const dy2 = square(Math.abs(y2 - y1));
  const dz2 = square(Math.abs(z2 - z1));
  return dx2 + dy2 + dz2;
};

class DSU {
  constructor(n) {
    this.parent = Array.from({ length: n }, (_, i) => i);
    this.sizes = Array(n).fill(1);
  }
  find(n) {
    if (this.parent[n] !== n) {
      this.parent[n] = this.find(this.parent[n]);
    }
    return this.parent[n];
  }
  union(a, b) {
    let rootA = this.find(a);
    let rootB = this.find(b);

    if (rootA === rootB) return false;

    if (this.sizes[rootA] < this.sizes[rootB]) {
      [rootA, rootB] = [rootB, rootA];
    }

    this.parent[rootB] = rootA;
    this.sizes[rootA] += this.sizes[rootB];
    return true;
  }

  getSubtrees() {
    const groups = new Map();
    for (let i = 0; i < this.parent.length; i++) {
      const root = this.find(i);
      if (!groups.has(root)) groups.set(root, []);
      groups.get(root).push(i);
    }
    return [...groups.values()].sort((a, b) => b.length - a.length);
  }
}

const kruskal = (n, edges, limit = edges.length) => {
  const dsu = new DSU(n);

  const mst = [];
  for (let i = 0; i < limit; i++) {
    const e = edges[i];
    if (dsu.union(e.a, e.b)) {
      mst.push(e);
    }
  }

  return { dsu, mst };
};
const partOne = (coords) => {
  const edges = [];
  for (let i = 0; i < coords.length; i++) {
    for (let j = i + 1; j < coords.length; j++) {
      edges.push({
        a: i,
        b: j,
        distance: distance(coords[i].coords, coords[j].coords),
      });
    }
  }
  edges.sort((a, b) => a.distance - b.distance);
  const { dsu } = kruskal(coords.length, edges, 1000);
  const trees = dsu.getSubtrees();

  return [0, 1, 2].reduce((x, i) => x * trees[i].length, 1);
};

const partTwo = (coords) => {
  const edges = [];
  for (let i = 0; i < coords.length; i++) {
    for (let j = i + 1; j < coords.length; j++) {
      edges.push({
        a: i,
        b: j,
        distance: distance(coords[i].coords, coords[j].coords),
      });
    }
  }
  edges.sort((a, b) => a.distance - b.distance);
  const { mst } = kruskal(coords.length, edges);
  const last = mst[mst.length - 1];
  const x = coords[last.a];
  const y = coords[last.b];
  return x.coords[0] * y.coords[0];
};

runner(parse, partOne, partTwo);
