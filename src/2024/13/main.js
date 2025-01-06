const filterMap = require('../../utils/filterMap');
const runner = require('../../utils/runner');

class Matrix {
  constructor(a, b, c, d) {
    this.a = a;
    this.b = b;
    this.c = c;
    this.d = d;
  }

  determinant() {
    return this.a * this.d - this.b * this.c;
  }

  multiplyInverse(vector) {
    const [x, y] = vector;
    const det = this.determinant();
    const x1 = (this.d * x - this.b * y) / det;
    const y1 = (this.a * y - this.c * x) / det;

    return [x1, y1];
  }
}

const parseMachine = (line) => {
  const [btnA, btnB, prize] = line.split('\n');
  const btnReg = /Button .: X\+(\d+), Y\+(\d+)/;
  const a = btnReg.exec(btnA);
  const x1 = Number(a[1]);
  const y1 = Number(a[2]);
  const b = btnReg.exec(btnB);
  const x2 = Number(b[1]);
  const y2 = Number(b[2]);
  const matrix = new Matrix(x1, x2, y1, y2);

  const prizeReg = /Prize: X=(\d+), Y=(\d+)/;
  const p = prizeReg.exec(prize);
  const px = Number(p[1]);
  const py = Number(p[2]);
  const vector = [px, py];
  return {
    matrix,
    vector,
  };
};

// A b = n => b = A^-1 n
const solve = (matrix, vector) => matrix.multiplyInverse(vector);

const parse = (input) => {
  const machines = [];
  const configs = input.split('\n\n');

  for (const cfg of configs) {
    const machine = parseMachine(cfg);
    if (machine !== null) machines.push(machine);
  }
  return machines;
};

const buttonACost = 3;
const buttonBCost = 1;

const sumCosts = (costs) =>
  costs.reduce((total, [a, b]) => total + a * buttonACost + b * buttonBCost, 0);

const partOne = (machines) => {
  const costs = filterMap(machines, ({ matrix, vector }) => {
    const [a, b] = solve(matrix, vector);
    return Number.isInteger(a) && Number.isInteger(b) ? [a, b] : null;
  });

  return sumCosts(costs);
};
const partTwo = (machines) => {
  const costs = filterMap(machines, ({ matrix, vector }) => {
    const [a, b] = solve(matrix, [
      vector[0] + 10000000000000,
      vector[1] + 10000000000000,
    ]);
    return Number.isInteger(a) && Number.isInteger(b) ? [a, b] : null;
  });

  return sumCosts(costs);
};

runner(parse, partOne, partTwo);
