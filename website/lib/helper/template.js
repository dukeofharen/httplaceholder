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

module.exports = {
    renderTemplate,
    renderTemplateAndWriteFile
};