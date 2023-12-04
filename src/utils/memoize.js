const memo = (fn, getK) => {
  const cache = new Map();
  return (...args) => {
    const k = getK(...args);
    if (cache.has(k)) {
      return cache.get(k);
    }
    const result = fn(...args);
    cache.set(k, result);
    return result;
  };
};

module.exports = memo;
