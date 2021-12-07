// Generated by ReScript, PLEASE EDIT WITH CARE
'use strict';

var Fs = require("fs");
var Path = require("path");
var Js_exn = require("rescript/lib/js/js_exn.js");
var Belt_Int = require("rescript/lib/js/belt_Int.js");
var Belt_Array = require("rescript/lib/js/belt_Array.js");

var initialPosition = [
  0,
  0
];

function evolve(param, direction) {
  var y = param[1];
  var x = param[0];
  switch (direction.TAG | 0) {
    case /* Forwards */0 :
        return [
                x + direction._0 | 0,
                y
              ];
    case /* Up */1 :
        return [
                x,
                y - direction._0 | 0
              ];
    case /* Down */2 :
        return [
                x,
                y + direction._0 | 0
              ];
    
  }
}

function multiply(param) {
  return Math.imul(param[0], param[1]);
}

function parseFileInstructions(filename) {
  var parseLine = function (line) {
    var match = line.split(" ");
    if (match.length !== 2) {
      return Js_exn.raiseError("Invalid line");
    }
    var direction = match[0];
    var amountStr = match[1];
    var amount = Belt_Int.fromString(amountStr);
    switch (direction) {
      case "down" :
          if (amount !== undefined) {
            return {
                    TAG: /* Down */2,
                    _0: amount
                  };
          } else {
            return Js_exn.raiseError("Could not parse line");
          }
      case "forward" :
          if (amount !== undefined) {
            return {
                    TAG: /* Forwards */0,
                    _0: amount
                  };
          } else {
            return Js_exn.raiseError("Could not parse line");
          }
      case "up" :
          if (amount !== undefined) {
            return {
                    TAG: /* Up */1,
                    _0: amount
                  };
          } else {
            return Js_exn.raiseError("Could not parse line");
          }
      default:
        return Js_exn.raiseError("Could not parse line");
    }
  };
  return Belt_Array.map(Fs.readFileSync(Path.resolve("src", "2021", "02", filename)).toString().split("\n"), parseLine);
}

function main(param) {
  var instructions = parseFileInstructions("data.txt");
  var finalPosition = Belt_Array.reduce(instructions, initialPosition, evolve);
  console.log("Part I: ", multiply(finalPosition));
  
}

var PartOne = {
  initialPosition: initialPosition,
  evolve: evolve,
  multiply: multiply,
  parseFileInstructions: parseFileInstructions,
  main: main
};

var initialPosition$1 = [
  0,
  0,
  0
];

function evolve$1(param, direction) {
  var z = param[2];
  var y = param[1];
  var x = param[0];
  switch (direction.TAG | 0) {
    case /* Forwards */0 :
        var v = direction._0;
        return [
                x + v | 0,
                y + Math.imul(z, v) | 0,
                z
              ];
    case /* Up */1 :
        return [
                x,
                y,
                z - direction._0 | 0
              ];
    case /* Down */2 :
        return [
                x,
                y,
                z + direction._0 | 0
              ];
    
  }
}

function multiply$1(param) {
  return Math.imul(param[0], param[1]);
}

function parseFileInstructions$1(filename) {
  var parseLine = function (line) {
    var match = line.split(" ");
    if (match.length !== 2) {
      return Js_exn.raiseError("Invalid line");
    }
    var direction = match[0];
    var amountStr = match[1];
    var amount = Belt_Int.fromString(amountStr);
    switch (direction) {
      case "down" :
          if (amount !== undefined) {
            return {
                    TAG: /* Down */2,
                    _0: amount
                  };
          } else {
            return Js_exn.raiseError("Could not parse line");
          }
      case "forward" :
          if (amount !== undefined) {
            return {
                    TAG: /* Forwards */0,
                    _0: amount
                  };
          } else {
            return Js_exn.raiseError("Could not parse line");
          }
      case "up" :
          if (amount !== undefined) {
            return {
                    TAG: /* Up */1,
                    _0: amount
                  };
          } else {
            return Js_exn.raiseError("Could not parse line");
          }
      default:
        return Js_exn.raiseError("Could not parse line");
    }
  };
  return Belt_Array.map(Fs.readFileSync(Path.resolve("src", "2021", "02", filename)).toString().split("\n"), parseLine);
}

function main$1(param) {
  var instructions = parseFileInstructions$1("data.txt");
  var finalPosition = Belt_Array.reduce(instructions, initialPosition$1, evolve$1);
  console.log("Part II: ", multiply$1(finalPosition));
  
}

var PartTwo = {
  initialPosition: initialPosition$1,
  evolve: evolve$1,
  multiply: multiply$1,
  parseFileInstructions: parseFileInstructions$1,
  main: main$1
};

main(undefined);

main$1(undefined);

exports.PartOne = PartOne;
exports.PartTwo = PartTwo;
/*  Not a pure module */
