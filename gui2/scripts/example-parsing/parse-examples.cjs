const { join } = require("path");
const {
  readdirSync,
  readFileSync,
  writeFileSync,
} = require("../helper/file.cjs");
const { load } = require("js-yaml");

const examplesRootDir = join(__dirname, "../../resources/examples");
const examples = readdirSync(examplesRootDir);
const parsedExamples = examples
  .map((e) => {
    return {
      filename: e,
      contents: readFileSync(join(examplesRootDir, e)).toString(),
    };
  })
  .map((e) => {
    const result = load(e.contents);
    result.id = e.filename.replace(".yml", "");
    return result;
  });

const exampleResultPath = join(
  __dirname,
  "../../src/assets/stub-examples.json",
);
writeFileSync(exampleResultPath, JSON.stringify(parsedExamples));
