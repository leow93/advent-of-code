const runner = require('../../utils/runner');
const fs = require('fs');
class Graph {
  g = new Map();
  addEdge(from, to) {
    this.g.set(from, (this.g.get(from) ?? new Set()).add(to));
  }
  get(node) {
    const neighbours = this.g.get(node);
    if (!neighbours) {
      console.log(`no neighbours found for ${node}`);
      return new Set().values();
    }
    return neighbours.values();
  }
}
const parse = (input) => {
  const graph = {};
  for (const line of input.split('\n')) {
    if (!line) continue;
    const [from, rest] = line.split(': ');
    const tos = rest.split(' ');

    graph[from] = (graph[from] ?? []).concat(tos);
  }
  return graph;
};

function toDot(graphData, graphName = 'MyGraph', isHorizontal = true) {
  const lines = [];

  // 1. Header
  lines.push(`digraph "${graphName}" {`);

  // 2. Graph Attributes
  if (isHorizontal) {
    lines.push('    rankdir=LR;');
  }
  lines.push('    node [shape=box fontname="Helvetica"];');
  lines.push('    edge [fontsize=10];');

  // Helper to escape double quotes in strings
  const escape = (str) => str.replace(/"/g, '\\"');

  // 3. Normalize input to an iterable entries array
  let entries;
  if (graphData instanceof Map) {
    entries = graphData.entries();
  } else {
    entries = Object.entries(graphData);
  }

  // 4. Generate Nodes and Edges
  for (const [source, targets] of entries) {
    const safeSource = escape(source);

    // Handle isolated nodes (keys with empty arrays)
    if (!targets || targets.length === 0) {
      lines.push(`    "${safeSource}";`);
      continue;
    }

    // Handle edges
    for (const target of targets) {
      const safeTarget = escape(target);
      lines.push(`    "${safeSource}" -> "${safeTarget}";`);
    }
  }

  // 5. Closer
  lines.push('}');

  return lines.join('\n');
}
runner(
  parse,
  (map) => fs.writeFileSync('graph', toDot(map)),
  () => {}
);
