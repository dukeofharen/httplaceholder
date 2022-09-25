const {join} = require("path");
const {exists, remove, ensureDir, copy} = require("./helper/file");
const {renderTemplateAndWriteFile, renderTemplate} = require("./helper/template");
const loadPosts = require("./loadPosts")

const devUrl = "";
const prodUrl = "";
const postPrefix = "HttPlaceholder";
const homePageTitle = "HttPlaceholder -  Quickly stub away any HTTP service";
const tutorialsPageTitle = "HttPlaceholder - tutorials";
const downloadsPageTitle = "HttPlaceholder - downloads";

const renderPage = async (distPath, rootUrl, htmlFileName, htmlDistFileName, pageTitle, extraProperties) => {
    const htmlDestinationPath = join(distPath, htmlDistFileName);
    let properties = {
        pageTitle: pageTitle,
        rootUrl
    };
    if (extraProperties) {
        properties = Object.assign(properties, extraProperties)
    }
    
    properties.body = await renderTemplate(htmlFileName, properties);
    await renderTemplateAndWriteFile("template.html", htmlDestinationPath, properties);
}

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
        await copy(staticPath, join(distPath, "static"));

        // Load and write posts.
        const posts = await loadPosts();
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
        await renderPage(distPath, rootUrl, "index.html", "index.html", homePageTitle);
        await renderPage(distPath, rootUrl, "download.html", "download.html", downloadsPageTitle);
        await renderPage(postsPath, rootUrl, "posts.html", "index.html", tutorialsPageTitle, {posts});
    } catch (e) {
        console.error(e);
        process.exit(1);
    }
})();