const mustache = require("mustache");
const {join} = require("path");
const {readFile, writeFile} = require("./file");

const renderTemplate = async (templateName, options) => {
    options = options || {};
    const templatePath = join(__dirname, `../../src/templates/${templateName}`);
    console.log(`Attempting to render template ${templatePath}.`);
    const template = await readFile(templatePath);
    return mustache.render(template, options);
};

const renderTemplateAndWriteFile = async (templateName, destinationPath, options) => {
    const renderedTemplate = await renderTemplate(templateName, options);
    await writeFile(destinationPath, renderedTemplate);
};

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

const render = async (distPath, rootUrl, templateInputFilename, outputFilename, extraProperties) => {
    const destinationPath = join(distPath, outputFilename);
    let properties = {
        rootUrl
    };
    if (extraProperties) {
        properties = Object.assign(properties, extraProperties)
    }

    const body = await renderTemplate(templateInputFilename, properties);
    await writeFile(destinationPath, body);
}

module.exports = {
    renderTemplate,
    renderTemplateAndWriteFile,
    renderPage,
    render
};