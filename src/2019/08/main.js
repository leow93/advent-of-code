const runner = require('../../utils/runner');

const parse = (x) => x.split('\n')[0].split('').map(Number);

const WIDTH = 25;
const HEIGHT = 6;

const newLayer = (width, height) =>
  Array.from({ length: height }).map(() => Array.from({ length: width }));

const decode = (width, height, data) => {
  const layers = [];
  let curr = newLayer(width, height);
  let w = 0;
  let h = 0;
  for (let i = 0; i < data.length; i++) {
    curr[h][w] = data[i];

    if (w === width - 1 && h === height - 1) {
      layers.push(curr);
      curr = newLayer(width, height);
      w = 0;
      h = 0;
      continue;
    }

    if (w === width - 1) {
      w = 0;
      h++;
    } else {
      w++;
    }
  }
  return layers;
};

const layerWithFewestZeroes = (layers) => {
  let min = Infinity;
  let id = -1;

  for (let i = 0; i < layers.length; i++) {
    const layer = layers[i];
    const zeroes = layer.reduce(
      (count, row) => count + row.filter((x) => x === 0).length,
      0
    );
    if (zeroes < min) {
      min = zeroes;
      id = i;
    }
  }

  return layers[id];
};

const partOne = (x) => {
  const layers = decode(WIDTH, HEIGHT, x);
  const layer = layerWithFewestZeroes(layers);
  let ones = 0;
  let twos = 0;
  for (let i = 0; i < layer.length; i++) {
    const row = layer[i];
    for (let j = 0; j < row.length; j++) {
      if (row[j] === 1) ones++;
      if (row[j] === 2) twos++;
    }
  }
  return ones * twos;
};
const partTwo = (x) => {
  const layers = decode(WIDTH, HEIGHT, x);
  const finalImage = newLayer(WIDTH, HEIGHT);

  for (let i = 0; i < finalImage.length; i++) {
    for (let j = 0; j < finalImage[i].length; j++) {
      for (let k = 0; k < layers.length; k++) {
        const col = layers[k][i][j];
        // transparent
        if (col === 2) continue;
        finalImage[i][j] = col === 0 ? ' ' : '#';
        break;
      }
    }
  }

  for (const row of finalImage) {
    console.log(row.join(''));
  }
};

runner(parse, partOne, partTwo);
