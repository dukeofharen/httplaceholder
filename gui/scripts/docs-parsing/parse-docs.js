/* eslint-disable no-undef */
const { join } = require("path");
const {
  exists,
  ensureDirSync,
  removeSync,
  copySync,
  readFileSync,
  writeFileSync,
} = require("../helper/file");
const { parse } = require("marked");
const { render } = require("mustache");

const publicRootDir = join(__dirname, "../../public");
const publicDocsRootDir = join(publicRootDir, "docs");
const docsRootDir = join(__dirname, "../../../docs");
const staticDir = join(__dirname, "static");

if (exists(publicDocsRootDir)) {
  console.log(`Removing directory ${publicDocsRootDir}.`);
  removeSync(publicDocsRootDir);
}

console.log(`Creating directory ${publicDocsRootDir}.`);
ensureDirSync(publicDocsRootDir);

console.log(`Copying contents from ${docsRootDir} to ${publicDocsRootDir}`);
copySync(docsRootDir, publicDocsRootDir);

console.log(`Copying contents from ${staticDir} to ${publicDocsRootDir}.`);
copySync(staticDir, publicDocsRootDir);

const docsMdPath = join(publicDocsRootDir, "docs.md");
const docsHtmlPath = join(publicDocsRootDir, "docs.html");
const docsTemplatePath = join(__dirname, "docs-template.html");
const docsMdContents = readFileSync(docsMdPath).toString();
const parsedDocs = parse(docsMdContents);
const docsTemplate = readFileSync(docsTemplatePath).toString();
const renderedDocs = render(docsTemplate, { contents: parsedDocs });
console.log(`Writing file ${docsHtmlPath}.`);
writeFileSync(docsHtmlPath, renderedDocs);

const indexHtmlTemplatePath = join(__dirname, "index.html");
const indexHtmlPath = join(publicDocsRootDir, "index.html");
const indexHtmlContents = readFileSync(indexHtmlTemplatePath).toString();
const renderedIndexHtml = render(docsTemplate, { contents: indexHtmlContents });
console.log(`Writing file ${indexHtmlPath}.`);
writeFileSync(indexHtmlPath, renderedIndexHtml);
