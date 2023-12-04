const readFromStdin = require('../../utils/readFromStdin');
const memo = require('../../utils/memoize');
const filterMap = require('../../utils/filterMap');

class ScratchCard {
  constructor(id, winningNumbers, drawnNumbers) {
    this.id = id;
    this.winningNumbers = winningNumbers;
    this.drawnNumbers = drawnNumbers;
  }

  static parse(id, winningNumbers, drawnNumbers) {
    function toStringArray(str) {
      return filterMap(str.trim().split(' '), (x) => {
        const trimmed = x.trim();
        if (trimmed === '') {
          return null;
        }
        return trimmed;
      });
    }

    return new ScratchCard(
      id,
      new Set(toStringArray(winningNumbers).map(Number)),
      new Set(toStringArray(drawnNumbers).map(Number))
    );
  }

  static fromLine(line) {
    const [idDef, cardDef] = line.split(':');
    const [winningNumbers, drawnNumbers] = cardDef.split('|');
    const id = idDef.replace(/Card\s+/, '');
    return ScratchCard.parse(id, winningNumbers, drawnNumbers);
  }
}

const intersection = (setA, setB) => {
  const result = new Set();
  const smaller = setA.size < setB.size ? setA : setB;
  const bigger = setA.size < setB.size ? setB : setA;
  for (const item of smaller) {
    if (bigger.has(item)) {
      result.add(item);
    }
  }
  return result;
};

const cardId = (card) => card.id;
const playCard = memo(
  (card) => intersection(card.winningNumbers, card.drawnNumbers).size,
  cardId
);

const partOne = (cards) =>
  cards.reduce((sum, card) => {
    const count = playCard(card);
    return count === 0 ? sum : sum + 2 ** (count - 1);
  }, 0);

const partTwo = (cards) => {
  const cardCounts = Object.fromEntries(cards.map((card) => [card.id, 1]));
  for (let i = 0; i < cards.length; i++) {
    const card = cards[i];
    const nCards = cardCounts[card.id];
    for (let j = 1; j <= nCards; j++) {
      const count = playCard(card);
      if (count > 0) {
        const ids = Array.from({ length: count }).map(
          (_, k) => Number(card.id) + 1 + k
        );

        for (const id of ids) {
          cardCounts[id] = (cardCounts[id] ?? 0) + 1;
        }
      }
    }
  }

  return Object.values(cardCounts).reduce((acc, x) => acc + x, 0);
};

const main = async () => {
  const data = await readFromStdin();
  const scratchCards = data.split('\n').map(ScratchCard.fromLine);
  console.log('Part one', partOne(scratchCards));
  console.log('Part two', partTwo(scratchCards));
};

main();
