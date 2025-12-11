const runner = require('../../utils/runner');
class Graph {
  g = new Map();
  addEdge(from, to) {
    this.g.set(from, (this.g.get(from) ?? new Set()).add(to));
  }
  get(node) {
    const neighbours = this.g.get(node);
    if (!neighbours) {
      return [];
    }
    return neighbours.values();
  }
}
const parse = (input) => {
  const graph = new Graph();
  for (const line of input.split('\n')) {
    if (!line) continue;
    const [from, rest] = line.split(': ');
    const tos = rest.split(' ');
    for (const to of tos) {
      graph.addEdge(from, to);
    }
  }
  return graph;
};
const dfs1 = (graph, start, end) => {
  let count = 0;
  const paths = new Set();
  const q = [{ node: start, path: start }];

  while (q.length) {
    const { node, path } = q.pop();
    if (paths.has(path)) continue;
    paths.add(path);
    if (node === end) {
      count++;
      continue;
    }

    for (const next of graph.get(node)) {
      q.push({ node: next, path: path + next });
    }
  }

  return count;
};
const dfs2 = (graph, ns, fft, dac, cache) => {
  let count = 0;
  for (const n of ns) {
    if (n === 'out') {
      return fft && dac ? 1 : 0;
    }

    let nfft = fft;
    let ndac = dac;
    const k = n + nfft + ndac;
    if (cache.has(k)) {
      count += cache.get(k);
      continue;
    }
    if (n === 'fft') nfft = true;
    if (n === 'dac') ndac = true;

    const x = dfs2(graph, graph.get(n), nfft, ndac, cache);
    cache.set(k, x);
    count += x;
  }
  return count;
};
const partOne = (graph) => dfs1(graph, 'you', 'out');
const partTwo = (graph) =>
  dfs2(graph, graph.get('svr'), false, false, new Map());
runner(parse, partOne, partTwo);
