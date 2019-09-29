let fs = require("fs");
let path = require("path");
let jsYaml = require("js-yaml");
const stubExamplesFilePath = path.join(__dirname, "../src/stub_examples.json");

fs.readdir(__dirname, (err, files) => {
    if (err) {
        console.error(err);
        return;
    }

    let result = [];
    let stubFiles = files.filter(f => f.indexOf(".yml") > -1);
    for (let file of stubFiles) {
        console.log(`Processing file ${file}`);
        let filePath = path.join(__dirname, file);
        let contents = fs.readFileSync(filePath, 'utf8');
        let stub = jsYaml.safeLoad(contents);
        result.push({
            key: stub.id,
            name: stub.description,
            stub: contents
        });
    }

    fs.writeFileSync(stubExamplesFilePath, JSON.stringify(result));
});