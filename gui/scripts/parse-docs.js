/* eslint-disable no-undef */
const { join } = require("path");
const {
  exists,
  ensureDirSync,
  removeSync,
  copySync,
} = require("./helper/file");

const publicRootDir = join(__dirname, "../public");
const publicDocsRootDir = join(publicRootDir, "docs");
const docsRootDir = join(__dirname, "../../docs");

if (exists(publicDocsRootDir)) {
  console.log(`Removing directory ${publicDocsRootDir}.`);
  removeSync(publicDocsRootDir);
}

console.log(`Creating directory ${publicDocsRootDir}.`);
ensureDirSync(publicDocsRootDir);

console.log(`Copying contents from ${docsRootDir} to ${publicDocsRootDir}`);
copySync(docsRootDir, publicDocsRootDir);
