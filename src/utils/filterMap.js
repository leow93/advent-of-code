const filterMap = (arr, fn) => {
  const result = [];
  for (const item of arr) {
    const mapped = fn(item);
    if (mapped != null) {
      result.push(mapped);
    }
  }
  return result;
};

module.exports = filterMap;
