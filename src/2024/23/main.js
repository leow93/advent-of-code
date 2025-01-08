const runner = require('../../utils/runner');

const parse = (input) => {
  const graph = {};
  for (const line of input.split('\n')) {
    if (!line || line === '\n') continue;

    const [a, b] = line.split('-');
    if (!graph[a]) graph[a] = new Set();
    if (!graph[b]) graph[b] = new Set();

    graph[a].add(b);
    graph[b].add(a);
  }

  return graph;
};

const getTriples = (graph) => {
  const triples = new Set();
  for (const node in graph) {
    const neighbours = Array.from(graph[node]);
    for (let i = 0; i < neighbours.length; i++) {
      for (let j = i + 1; j < neighbours.length; j++) {
        const [n1, n2] = [neighbours[i], neighbours[j]];

        // Check if these two neighbors are connected to each other
        if (graph[n1].has(n2)) {
          const triple = [node, n1, n2].sort().join(',');
          triples.add(triple);
        }
      }
    }
  }

  return Array.from(triples);
};

// Check if a given set of nodes forms a clique (fully connected)
const isClique = (nodes, graph) => {
  for (let i = 0; i < nodes.length; i++) {
    for (let j = i + 1; j < nodes.length; j++) {
      if (!graph[nodes[i]].has(nodes[j])) {
        return false;
      }
    }
  }
  return true;
};

// Find the largest clique in the graph
const findLargestClique = (graph) => {
  const nodes = Object.keys(graph);
  let largestClique = [];
  let i = 0;

  // Generate all subsets of nodes (2^n subsets)
  const generateSubsets = (current, remaining) => {
    i++;
    if (remaining.length === 0) {
      if (current.length > largestClique.length && isClique(current, graph)) {
        largestClique = current;
      }
      return;
    }

    const [first, ...rest] = remaining;

    // Include the first node in the subset
    generateSubsets([...current, first], rest);

    // Exclude the first node from the subset
    generateSubsets(current, rest);
  };

  generateSubsets([], nodes);
  return largestClique;
};

const findMaximalCliques = (graph) => {
  const maximalCliques = [];

  const bronKerbosch = (r, p, x) => {
    if (p.size === 0 && x.size === 0) {
      maximalCliques.push([...r]);
      return;
    }

    const pArray = Array.from(p);
    for (const v of pArray) {
      const neighbors = graph[v] || new Set();
      bronKerbosch(
        new Set([...r, v]),
        new Set([...p].filter((n) => neighbors.has(n))),
        new Set([...x].filter((n) => neighbors.has(n)))
      );
      p.delete(v);
      x.add(v);
    }
  };

  const nodes = Object.keys(graph);
  bronKerbosch(new Set(), new Set(nodes), new Set());
  return maximalCliques;
};

const partOne = (graph) => {
  const triples = getTriples(graph);
  let count = 0;

  for (const triple of triples) {
    const t = triple.split(',');
    if (t.some((x) => x.startsWith('t'))) {
      count++;
    }
  }

  return count;
};
const partTwo = (data) => {
  const largestCliques = findMaximalCliques(data);
  const largestClique = largestCliques.reduce(
    (max, clique) => (clique.length > max.length ? clique : max),
    []
  );

  return largestClique.sort().join(',');
};

runner(parse, partOne, partTwo);
