/**
 * Very useful for piping data into a script e.g.
 * cat input.txt | node main.js
 * @returns {Promise<string>}
 */
const readFromStdin = async () => {
  const chunks = [];
  for await (const chunk of process.stdin) {
    chunks.push(chunk);
  }
  return Buffer.concat(chunks).toString();
};

module.exports = readFromStdin;
