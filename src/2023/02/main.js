const rawData = require('fs').readFileSync('./data.txt', 'utf8');

const parseId = (gameX) => {
  const id = gameX.split('Game ')[1];
  if (!id) throw new Error('Invalid game id');
  return Number(id);
};

const parseCubeCount = (line) => {
  const regex = /(\d+) (\D+)/i;
  const results = line.match(regex);
  const count = Number(results[1]);
  const colour = results[2];
  switch (colour) {
    case 'red':
      return { reds: count, blues: 0, greens: 0 };
    case 'blue':
      return { reds: 0, blues: count, greens: 0 };
    case 'green':
      return { reds: 0, blues: 0, greens: count };
    default:
      throw new Error('Invalid colour');
  }
};

const parseRound = (roundStr) =>
  roundStr
    .trim()
    .split(',')
    .reduce(
      (state, line) => {
        const { reds, blues, greens } = parseCubeCount(line);
        return {
          reds: state.reds + reds,
          blues: state.blues + blues,
          greens: state.greens + greens,
        };
      },
      { reds: 0, blues: 0, greens: 0 }
    );

const parsedData = rawData.split('\n').map((line) => {
  const [gameX, rounds] = line.split(':');
  const id = parseId(gameX);
  const roundsArray = rounds.split(';').map(parseRound);

  return {
    id,
    rounds: roundsArray,
  };
});

const findPossibleGames = (games, { totalReds, totalGreens, totalBlues }) => {
  return games.filter((game) =>
    game.rounds.every(
      (round) =>
        round.reds <= totalReds &&
        round.greens <= totalGreens &&
        round.blues <= totalBlues
    )
  );
};

const partOne = (games, totalCubes) => {
  const possibleGames = findPossibleGames(games, totalCubes);
  return possibleGames.reduce((acc, game) => acc + game.id, 0);
};

console.log(
  'Part I:',
  partOne(parsedData, { totalReds: 12, totalBlues: 14, totalGreens: 13 })
);

const partTwo = (games) => {
  const maxBy = (arr, key) => arr.sort((a, b) => b[key] - a[key])[0][key];

  return games.reduce((total, game) => {
    const maxReds = maxBy(game.rounds, 'reds');
    const maxBlues = maxBy(game.rounds, 'blues');
    const maxGreens = maxBy(game.rounds, 'greens');
    return total + maxReds * maxBlues * maxGreens;
  }, 0);
};
console.log('Part II:', partTwo(parsedData));
