const {join} = require("path");
const {readdir, readFile, exists, remove, ensureDir, copy} = require("./helper/file");
const dayjs = require("dayjs");
const marked = require("marked");
const {renderTemplateAndWriteFile, renderTemplate} = require("./helper/template");

const prodUrl = "https://httplaceholder.org";
const postPrefix = "HttPlaceholder";
const homePageTitle = "HttPlaceholder -  Quickly stub away any HTTP service";
const tutorialsPageTitle = "HttPlaceholder - tutorials";

const loadPosts = async () => {
    const postsPath = join(__dirname, "../src/posts");
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

(async () => {
    try {
        // Determine root URL.
        const rootUrl = process.argv.length > 2 && process.argv[2] === "dev" ? "http://localhost:8080" : prodUrl;

        const distPath = join(__dirname, "../dist");
        const srcPath = join(__dirname, "../src");
        const staticPath = join(srcPath, "static");

        // Rebuild dist directory.
        if (await exists(distPath)) {
            await remove(distPath);
        }

        await ensureDir(distPath);

        // Copy static files to dist.
        await copy(staticPath, join(distPath, "static"));

        // Load and write posts.
        const posts = await loadPosts();
        const postsPath = join(distPath, "posts");
        for (const post of posts) {
            const postFolderPath = join(postsPath, post.postKey);
            await ensureDir(postFolderPath);
            const postPath = join(postFolderPath, "index.html");
            const renderedBlogPost = await renderTemplate("blog.html", {
                title: post.title,
                contents: post.markdown,
                date: post.formattedDate,
                rootUrl
            });
            await renderTemplateAndWriteFile("template.html", postPath, {
                pageTitle: `${postPrefix} - ${post.title}`,
                body: renderedBlogPost,
                rootUrl
            });
        }

        // Load and write home page.
        const postCap = 5;
        const indexPath = join(distPath, "index.html");
        const renderedHomePage = await renderTemplate("index.html", {
            posts: posts.slice(0, postCap),
            rootUrl
        });
        await renderTemplateAndWriteFile("template.html", indexPath, {
            pageTitle: homePageTitle,
            body: renderedHomePage,
            rootUrl
        });

        // Load and write posts page.
        const postsPagePath = join(postsPath, "index.html");
        const renderedPostsPage = await renderTemplate("posts.html", {
            posts,
            rootUrl
        });
        await renderTemplateAndWriteFile("template.html", postsPagePath, {
            pageTitle: tutorialsPageTitle,
            body: renderedPostsPage,
            rootUrl
        });
    } catch (e) {
        console.error(e);
        process.exit(1);
    }
})();