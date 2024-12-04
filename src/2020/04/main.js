const runner = require('../../utils/runner');

class Map {
  constructor(data) {
    this.data = data ?? {};
  }
  set(k, v) {
    return new Map({ ...this.data, [k]: v });
  }
  get(k) {
    return this.data[k];
  }
  has(k) {
    return k in this.data;
  }
}

const parse = (data) => {
  const rows = data.split('\n\n');
  return rows.map((row) => {
    return row
      .replaceAll('\n', ' ')
      .split(' ')
      .reduce((map, field) => {
        const parts = field.split(':');
        return map.set(parts[0], parts[1]);
      }, new Map());
  });
};

const run = (validity) => (passports) => {
  let count = 0;
  for (const p of passports) {
    if (validity(p)) {
      count++;
    }
  }
  return count;
};

const hasRequiredFields = (passport) =>
  ['byr', 'iyr', 'eyr', 'hgt', 'hcl', 'ecl', 'pid'].every((key) =>
    passport.has(key)
  );

const yearValidator = (key, min, max) => (pp) => {
  const x = pp.get(key);
  if (!x) return false;
  if (x.length !== 4) return false;
  const value = parseInt(x);
  return value >= min && value <= max;
};

const validBirthYear = yearValidator('byr', 1920, 2002);
const validIssueYear = yearValidator('iyr', 2010, 2020);
const validExpYear = yearValidator('eyr', 2020, 2030);
const validHeight = (pp) => {
  const h = pp.get('hgt');
  if (!h) return false;
  const unit = h.at(-2) + h.at(-1);
  const valueStr = h.slice(0, h.length - 2);
  const value = parseInt(valueStr);
  switch (unit) {
    case 'cm':
      return value >= 150 && value <= 193;
    case 'in':
      return value >= 59 && value <= 76;
    default:
      return false;
  }
};
const validHairColour = (pp) => {
  const hcl = pp.get('hcl');
  if (!hcl) return false;
  return hcl.startsWith('#') && /^[0-9a-f]{6}$/.test(hcl.slice(1));
};
const validEyeColour = (pp) => {
  const ecl = pp.get('ecl');
  if (!ecl) return false;
  return ['amb', 'blu', 'brn', 'gry', 'grn', 'hzl', 'oth'].includes(ecl);
};
const validPPId = (pp) => /^[0-9]{9}$/.test(pp.get('pid'));

const and =
  (...fs) =>
  (x) => {
    for (const f of fs) {
      if (!f(x)) {
        return false;
      }
    }
    return true;
  };

const fieldsAreValid = and(
  validBirthYear,
  validIssueYear,
  validExpYear,
  validHeight,
  validHairColour,
  validEyeColour,
  validPPId
);
const one = run(hasRequiredFields);

const two = run(fieldsAreValid);

const main = () => runner(parse, one, two);

main();
