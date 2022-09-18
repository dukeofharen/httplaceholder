const dayjs = require("dayjs");
const {readline} = require("../lib/helper/readline");
const {join} = require("path");
const {writeFile, exists} = require("../lib/helper/file");

(async () => {
    let title;
    if (process.argv.length !== 3) {
        title = await readline("Please fill in a post title: ");
    } else {
        title = process.argv[2];
    }

    const now = dayjs();
    const nowFormatted = now.format("YYYY-MM-DDTHH:mm:ssZ");
    const template = `---
title: ${title}
date: ${nowFormatted}
description: ${title}
---

<!-- Content here -->`;

    const postKey = title.toLowerCase()
        .replace(/[^a-zA-Z0-9]/gi, " ")
        .replace(/\s+/g, "-");
    const postFilename = `${postKey}.md`;

    const postPath = join(__dirname, "../src/posts", postFilename);
    if (await exists(postPath)) {
        console.error(`File "${postPath}" already exists. Specify other post name.`);
    } else {
        console.log(`Writing new post to "${postPath}"`);
        await writeFile(postPath, template);
    }
})();