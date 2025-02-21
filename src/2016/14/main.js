const crypto = require('crypto');
const runner = require('../../utils/runner');

const parse = (data) =>
  data
    .split('')
    .filter((x) => x !== '\n')
    .join('');

class Cache {
  constructor(limit = 50) {
    if (limit <= 0) {
      throw new Error('Limit must be a positive number');
    }
    this.limit = limit;
    this.cache = new Map();
  }

  // Sets a key-value pair in the cache
  set(key, value) {
    if (this.cache.size > this.limit) {
      this.clear();
    }

    this.cache.set(key, value);
    return this;
  }

  // Gets a value by key
  get(key) {
    if (!this.cache.has(key)) {
      return undefined; // Return undefined if the key is not in the cache
    }
    const value = this.cache.get(key);
    this.cache.delete(key); // Remove the key to update its order
    this.cache.set(key, value); // Re-insert the key to mark it as recently used
    return value;
  }

  // Deletes a key-value pair by key
  delete(key) {
    return this.cache.delete(key);
  }

  // Checks if the cache contains a key
  has(key) {
    return this.cache.has(key);
  }

  // Clears the cache
  clear() {
    this.cache.clear();
  }

  // Returns the current size of the cache
  get size() {
    return this.cache.size;
  }

  // Returns an iterator of the cache's keys
  keys() {
    return this.cache.keys();
  }

  // Returns an iterator of the cache's values
  values() {
    return this.cache.values();
  }

  // Returns an iterator of the cache's entries ([key, value] pairs)
  entries() {
    return this.cache.entries();
  }

  // Executes a callback function for each key-value pair in the cache
  forEach(callback, thisArg) {
    for (const [key, value] of this.cache) {
      callback.call(thisArg, value, key, this);
    }
  }
}

const cache = new Cache(10_000_000);

const md5 = (string) => {
  if (cache.has(string)) return cache.get(string);
  const hash = crypto.createHash('md5');
  hash.update(string);
  const result = hash.digest('hex').toLowerCase();
  cache.set(string, result);
  return result;
};

const cache2 = new Map();
const md5Loop = (string) => {
  if (cache2.has(string)) return cache2.get(string);
  let idx = 0;
  let hash = string;
  while (idx < 2017) {
    hash = md5(hash);
    idx++;
  }
  cache2.set(string, hash);
  return hash;
};

const sameChars = (string) => {
  for (let i = 0; i < string.length - 1; i++) {
    if (string[i] !== string[i + 1]) {
      return false;
    }
  }
  return true;
};

const findTriplet = (input) => {
  for (let i = 0; i < input.length - 3; i++) {
    const substring = input.slice(i, i + 3);
    if (sameChars(substring)) {
      return substring[0];
    }
  }
  return null;
};

const findPentuplet = (input, search) => {
  for (let i = 0; i < input.length - 5; i++) {
    if (input[i] !== search) continue;
    const substring = input.slice(i, i + 5);
    if (sameChars(substring)) {
      return substring[0];
    }
  }
  return null;
};

const run = (hashFn) => (input) => {
  let idx = 0;
  let count = 0;
  while (true) {
    const hash = hashFn(input + idx);
    const triplet = findTriplet(hash);
    if (triplet === null) {
      idx++;
      continue;
    }

    for (let i = idx + 1; i <= idx + 1000; i++) {
      const next = hashFn(input + i);
      const pentuplet = findPentuplet(next, triplet);
      if (pentuplet !== null) {
        count++;
        console.log({ count });
        if (count === 63) return idx;
      }
    }
    idx++;
  }
};

runner(parse, run(md5), run(md5Loop));
