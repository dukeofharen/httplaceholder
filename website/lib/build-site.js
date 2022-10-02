const {join} = require("path");
const {exists, remove, ensureDir, copy} = require("./helper/file");
const {renderPage} = require("./helper/template");
const loadPosts = require("./helper/loadPosts");
const parseChangelog = require("./helper/changelog");

const devUrl = "";
const prodUrl = "";
const postPrefix = "HttPlaceholder";
const homePageTitle = "HttPlaceholder - Quickly stub away any HTTP service";
const tutorialsPageTitle = "HttPlaceholder - tutorials";
const downloadsPageTitle = "HttPlaceholder - downloads";
const changelogPageTitle = "HttPlaceholder - changelog";

(async () => {
    try {
        // Determine root URL.
        const rootUrl = process.argv.length > 2 && process.argv[2] === "dev" ? devUrl : prodUrl;

        const distPath = join(__dirname, "../dist");
        const srcPath = join(__dirname, "../src");
        const staticPath = join(srcPath, "static");

        // Rebuild dist directory.
        if (await exists(distPath)) {
            await remove(distPath);
        }

        await ensureDir(distPath);

        // Copy static files to dist.
        await copy(`${staticPath}/.`, distPath);

        // Load and write posts.
        const posts = await loadPosts();
        const homepagePosts = posts.slice(0, 5);
        const postsPath = join(distPath, "posts");
        for (const post of posts) {
            const postFolderPath = join(postsPath, post.postKey);
            await ensureDir(postFolderPath);

            await renderPage(postFolderPath, rootUrl, "blog.html", "index.html", `${postPrefix} - ${post.title}`, {
                title: post.title,
                contents: post.markdown,
                date: post.formattedDate
            });
        }

        // Load and write pages.
        await renderPage(distPath, rootUrl, "index.html", "index.html", homePageTitle, {posts: homepagePosts, hasPosts: !!homepagePosts.length});
        await renderPage(distPath, rootUrl, "download.html", "download.html", downloadsPageTitle);
        if (posts.length) {
            await renderPage(postsPath, rootUrl, "posts.html", "index.html", tutorialsPageTitle, {posts});
        }

        const changelog = await parseChangelog();
        await renderPage(distPath, rootUrl, "changelog.html", "changelog.html", changelogPageTitle, {changelog})
    } catch (e) {
        console.error(e);
        process.exit(1);
    }
})();