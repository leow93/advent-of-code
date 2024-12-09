const runner = require('../../utils/runner');

const makeFileBlock = (id) => ({ id, type: 'file' });
const makeFreeBlock = (id) => ({ type: 'free' });

const parse = (line) => {
  const result = [];

  let id = 0;

  for (let i = 0; i < line.length; i++) {
    const mode = i % 2 === 0 ? 'file' : 'space';
    const n = Number(line[i]);
    if (mode === 'file') {
      const blocks = Array.from({ length: n }).map(() => makeFileBlock(id));
      result.push(...blocks);
      id++;
    } else {
      const blocks = Array.from({ length: n }).map(() => makeFreeBlock());
      result.push(...blocks);
    }
  }
  return result;
};

const findLastIdx = (arr, search) => {
  for (let i = arr.length - 1; i >= 0; i--) {
    if (search(arr[i])) return i;
  }
  return null;
};

const moveSingleBlock = (input) => {
  const arr = input.slice();
  const lastChIdx = findLastIdx(arr, (x) => x.type === 'file');
  const firstEmptyIdx = arr.findIndex((x) => x.type === 'free');
  arr[firstEmptyIdx] = arr[lastChIdx];
  arr[lastChIdx] = makeFreeBlock();

  return arr;
};

const sorted = (input) => {
  const lastFile = findLastIdx(input, (x) => x.type === 'file');
  const firstFree = input.findIndex((x) => x.type === 'free');
  return firstFree > lastFile;
};

const checksum = (input) => {
  let result = 0;
  for (let i = 0; i < input.length; i++) {
    const block = input[i];
    if (block.type === 'free') continue;

    result += i * Number(block.id);
  }
  return result;
};

const partOne = (input) => {
  let data = input.slice();
  while (!sorted(data)) {
    data = moveSingleBlock(data);
  }
  return checksum(data);
};

const findRange = (data, id) => {
  let start, length;

  for (let i = 0; i < data.length; i++) {
    const block = data[i];
    if (start == undefined && block.id === id) {
      start = i;
      length = 1;
      continue;
    }

    if (start !== undefined) {
      if (block.id === id) {
        length++;
        continue;
      } else {
        return { start, length };
      }
    }
  }

  return { start, length };
};

const freeBlocks = (data, before) => {
  const blocks = [];
  let curr;
  for (let i = 0; i < before; i++) {
    const b = data[i];
    if (b.type === 'free') {
      if (!curr) {
        curr = { start: i, length: 0 };
      }
      curr.length++;
      continue;
    }
    if (b.type === 'file') {
      if (curr) {
        blocks.push(curr);
        curr = undefined;
      }
    }
  }
  if (curr) {
    blocks.push(curr);
  }
  return blocks;
};

const partTwo = (input) => {
  let data = input.slice();
  const maxId = Math.max(
    ...data.filter((x) => x.type === 'file').map((x) => x.id)
  );
  for (let id = maxId; id >= 0; id--) {
    const blockRange = findRange(data, id);
    const free = freeBlocks(data, blockRange.start);
    const freeBlock = free.find((b) => b.length >= blockRange.length);
    if (!freeBlock) continue;

    for (
      let j = freeBlock.start;
      j < freeBlock.start + blockRange.length;
      j++
    ) {
      data[j] = makeFileBlock(id);
    }
    for (
      let j = blockRange.start;
      j < blockRange.start + blockRange.length;
      j++
    ) {
      data[j] = makeFreeBlock();
    }
  }

  return checksum(data);
};

if (require.main === module) {
  runner(parse, partOne, partTwo);
}
