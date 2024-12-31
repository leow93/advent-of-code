const { parse, reducer, format, sumGPSCoords } = require('./part2_internal');

const run = (input) => {
  const { instructions, ...state } = parse(input);
  console.log('initial');
  console.log(format(state));
  return instructions.reduce(reducer, state);
};

//  const input = `#######
//#...#.#
//#.....#
//#..OO@#
//#..O..#
//#.....#
//#######
//
//<vv<<^^<<^^`;

it('can move left', () => {
  const input = `#######
#...#.#
#.....#
#..OO@#
#..O..#
#.....#
#######

<<<<<`;

  const result = run(input);

  expect(format(result)).toEqual(`##############
##......##..##
##..........##
##[][]@.....##
##....[]....##
##..........##
##############`);
});

it('only moves contiguous boxes left', () => {
  const input = `#######
#...#.#
#.....#
#O..O@#
#..O..#
#.....#
#######

<`;

  const result = run(input);
  expect(format(result)).toEqual(`##############
##......##..##
##..........##
##[]...[]@..##
##....[]....##
##..........##
##############`);
});

it('can move right', () => {
  const input = `#######
#...#.#
#.....#
#.@OO.#
#..O..#
#.....#
#######

>>>>`;

  const result = run(input);

  expect(format(result)).toEqual(`##############
##......##..##
##..........##
##.....@[][]##
##....[]....##
##..........##
##############`);
});

it('can move down', () => {
  const input = `#######
#...#.#
#..@..#
#..OO.#
#..O..#
#.....#
#######

vv`;

  const result = run(input);

  expect(format(result)).toEqual(`##############
##......##..##
##..........##
##....@.[]..##
##....[]....##
##....[]....##
##############`);
});

it('can move down, complex', () => {
  const input = `#######
#...#.#
#..@..#
#..OO.#
#..O..#
#.OO..#
#.O...#
#.....#
#.....#
#######

<vv<<v><v^^^^>>>v>^>vvvv`;

  const result = run(input);
  expect(format(result)).toEqual(`##############
##......##..##
##..........##
##..........##
##....[]....##
##....[]....##
##...[].@...##
##..[]..[]..##
##.....[]...##
##############`);
});

it('can move up', () => {
  const input = `#######
#...#.#
#.....#
#..OO.#
#..O..#
#..@..#
#######

^`;

  const result = run(input);

  expect(format(result)).toEqual(`##############
##......##..##
##....[]....##
##....[][]..##
##....@.....##
##..........##
##############`);
});

it('can move up, complex', () => {
  const input = `#######
#...#.#
#.....#
#.@OO.#
#..O..#
#.OO..#
#.O...#
#.....#
#.....#
#######

>><<<vvv>v>^`;

  const result = run(input);
  expect(format(result)).toEqual(`##############
##......##..##
##.....[]...##
##....[].[].##
##..[][]....##
##...[].....##
##...@......##
##..........##
##..........##
##############`);
});

it('works on the test data', () => {
  const input = `##########
#..O..O.O#
#......O.#
#.OO..O.O#
#..O@..O.#
#O#..O...#
#O..O..O.#
#.OO.O.OO#
#....O...#
##########

<vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^
vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v
><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<
<<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^
^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><
^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^
>^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^
<><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>
^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>
v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^`;

  const result = run(input);
  expect(format(result)).toEqual(`####################
##[].......[].[][]##
##[]...........[].##
##[]........[][][]##
##[]......[]....[]##
##..##......[]....##
##..[]............##
##..@......[].[][]##
##......[][]..[]..##
####################`);
  expect(sumGPSCoords(result)).toEqual(9021);
});

it('works on the simple test data', () => {
  const input = `#######
#...#.#
#.....#
#..OO@#
#..O..#
#.....#
#######

<vv<<^^<<^^`;

  const result = run(input);
  expect(format(result)).toEqual(`##############
##...[].##..##
##...@.[]...##
##....[]....##
##..........##
##..........##
##############`);

  expect(sumGPSCoords(result)).toEqual(618);
});

it('fixes the bug', () => {
  const input = `######
#....#
#.O..#
#.OO@#
#.O..#
#....#
######

<v<v<^`;

  expect(format(run(input))).toEqual(`############
##..[]....##
##.[][]...##
##..[]....##
##...@....##
##........##
############`);
});
