const {join} = require("path");
const {readFile, writeFile} = require("./helper/file");
const cheerio = require("cheerio");

(async () => {
    const docsIndexPath = join(__dirname, "..", "dist", "docs", "index.html");
    const docsIndexContents = await readFile(docsIndexPath);

    const docsIndex = cheerio.load(docsIndexContents);
    docsIndex('head').append('<script defer data-domain="httplaceholder.org" src="https://stats.ducode.org/js/plausible.js"></script>');
    await writeFile(docsIndexPath, docsIndex.html());
})();