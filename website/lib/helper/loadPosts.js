const {join} = require("path");
const {readdir, readFile} = require("./file");
const dayjs = require("dayjs");
const marked = require("marked");

const loadPosts = async () => {
    const postsPath = join(__dirname, "../../src/posts");
    const posts = (await readdir(postsPath))
        .filter(f => f.endsWith(".md"));
    const result = [];
    const titleRegex = /^title: (.*)$/m;
    const dateRegex = /^date: (.*)$/m;
    const descriptionRegex = /^description: (.*)$/m;
    for (let post of posts) {
        const postPath = join(postsPath, post);
        const fileContents = await readFile(postPath);
        const titleMatches = fileContents.match(titleRegex);
        if (!titleMatches) {
            return Promise.reject(`No title set for post ${post}`);
        }

        const title = titleMatches[1];
        const dateMatches = fileContents.match(dateRegex);
        if (!dateMatches) {
            return Promise.reject(`No date set for post ${post}`);
        }

        const rawDate = dateMatches[1];
        const descriptionMatches = fileContents.match(descriptionRegex);
        if (!descriptionMatches) {
            return Promise.reject(`No description set for post ${post}`);
        }

        const description = descriptionMatches[1];

        const date = dayjs(rawDate);
        const cleanedPost = fileContents.split("---")[2].trim();
        result.push({
            postKey: post.replace(".md", ""),
            title,
            date,
            description,
            formattedDate: date.format("YYYY-MM-DD HH:mm"),
            formattedDateShort: date.format("MMM DD 'YY"),
            formattedDateRss: date.format("ddd, DD MMM YYYY"),
            cleanedPost,
            markdown: marked.marked(cleanedPost)
        })
    }

    result.sort((a, b) => {
        const unixA = a.date.unix();
        const unixB = b.date.unix();
        if (unixA > unixB) return -1;
        if (unixA < unixB) return 1;
        return 0;
    });
    return result;
};

module.exports = loadPosts