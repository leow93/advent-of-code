const map = {
  Q: ['O', 'L', 'B', 'S', 'V'],
  Z: ['G', 'O', 'F', 'J', 'A', 'X', 'T', 'P', 'U'],
  W: ['V', 'X', 'K', 'G'],
  C: ['X', 'E', 'P'],
  O: ['E', 'P', 'R', 'N', 'T'],
  K: ['N', 'B', 'V'],
  P: ['I', 'T', 'D', 'R', 'B', 'X', 'L'],
  X: ['D', 'A', 'V'],
  N: ['E', 'R', 'T', 'Y', 'G', 'D'],
  F: ['A', 'B', 'G'],
  U: ['Y', 'R', 'M', 'I', 'T', 'A'],
  M: ['H', 'L', 'Y', 'D'],
  J: ['B', 'E', 'H', 'G', 'T', 'Y', 'R'],
  B: ['E', 'V', 'R', 'Y', 'G'],
  S: ['L', 'E', 'D', 'T', 'G', 'R'],
  A: ['L', 'I'],
  E: ['L', 'T', 'Y'],
  L: ['G', 'V', 'H'],
  D: ['I', 'Y', 'V'],
  Y: ['I', 'G', 'R'],
  I: ['G', 'H', 'R'],
  G: ['R', 'V'],
  V: ['T', 'R', 'H'],
  R: ['H', 'T'],
  H: ['T'],
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

console.log(toDot(map));
