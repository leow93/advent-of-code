const rotLeft = (str, n = 1) => {
  const len = str.length;
  const idxs = Array.from({ length: str.length }).map((_, i) => {
    const idx = i + n;
    return ((idx % len) + len) % len;
  });

  let result = '';
  for (let i = 0; i < idxs.length; i++) {
    result += str[idxs[i]];
  }
  return result;
};

rotLeft('abc');
rotLeft('abc', 2);
rotLeft('abc', 3);
rotLeft('abcdef');
