const { join } = require("path");
const { readdirSync, readFileSync, writeFileSync } = require("../helper/file");
const { load } = require("js-yaml");

const examplesRootDir = join(__dirname, "../../resources/examples");
const examples = readdirSync(examplesRootDir);
const parsedExamples = examples
  .map((e) => readFileSync(join(examplesRootDir, e)).toString())
  .map((e) => load(e));

const exampleResultPath = join(
  __dirname,
  "../../src/constants/stub-examples.json"
);
writeFileSync(exampleResultPath, JSON.stringify(parsedExamples));
