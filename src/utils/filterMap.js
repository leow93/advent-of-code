const filterMap = (arr, fn) => {
  const result = [];
  for (let i = 0; i < arr.length; i++) {
    const item = arr[i];
    const mapped = fn(item, i);

    if (mapped != null) {
      result.push(mapped);
    }
  }
  return result;
};

module.exports = filterMap;
