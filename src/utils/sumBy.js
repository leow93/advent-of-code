const sumBy = (f) => (xs) => {
  let count = 0;
  for (const x of xs) {
    count += f(x);
  }
  return count;
};

module.exports = sumBy;
