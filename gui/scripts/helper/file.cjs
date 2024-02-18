/* eslint-disable no-undef */
const fsPromises = require("fs/promises");
const { readFileSync, writeFileSync, readdirSync } = require("fs");
const { copySync, exists, ensureDirSync, removeSync } = require("fs-extra");

const readFile = async (filePath) => {
  return (await fsPromises.readFile(filePath)).toString();
};

module.exports = {
  readFile,
  copySync,
  exists,
  ensureDirSync,
  removeSync,
  writeFile: fsPromises.writeFile,
  readdir: fsPromises.readdir,
  readdirSync,
  readFileSync,
  writeFileSync,
};
