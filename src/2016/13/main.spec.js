class Grid {
  constructor() {
    this.data = [];
  }
  set(y, x, value) {
    while (this.data.length <= y) {
      this.data.push(Array.from({ length: x }).fill('.'));
    }

    const data = this.data.map((row) => {
      const r = row.slice();
      while (r.length <= x) {
        r.push('.');
      }
      return r;
    });

    data[y][x] = value;
    this.data = data;
  }
  print() {
    for (const row of this.data) {
      console.log(row.join(''));
    }
  }
}

it('can set arbitary points', () => {
  const g = new Grid();
  g.set(1, 1, 'O');
  expect(g.data).toEqual([
    ['.', '.'],
    ['.', 'O'],
  ]);

  g.set(3, 3, '#');
  expect(g.data).toEqual([
    ['.', '.', '.', '.'],
    ['.', 'O', '.', '.'],
    ['.', '.', '.', '.'],
    ['.', '.', '.', '#'],
  ]);

  g.set(2, 2, '#');

  expect(g.data).toEqual([
    ['.', '.', '.', '.'],
    ['.', 'O', '.', '.'],
    ['.', '.', '#', '.'],
    ['.', '.', '.', '#'],
  ]);
});
