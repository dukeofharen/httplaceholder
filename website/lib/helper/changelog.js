const {join} = require("path");
const {readFile} = require("./file");
const marked = require("marked");

const finalizeVersions = (versions) => {
    versions = versions.filter(v => v.version !== "vnext");
    for (const version of versions) {
        const rawContents = version.lines.join("\n").trim();
        version.contents = marked.marked(rawContents);
        delete version.lines;
    }
    
    return versions;
};

const parseChangelog = async () => {
    const changelogPath = join(__dirname, "..", "..", "..", "CHANGELOG");
    const changelog = await readFile(changelogPath);
    const lines = changelog.split(/\r?\n|\r|\n/g);
    let versions = [];
    let currentVersion;
    for (let line of lines) {
        if (line.startsWith("[")) {
            if (currentVersion) {
                versions.push(currentVersion);
            }

            currentVersion = {};
            currentVersion.version = line.replace("[", "").replace("]", "");
            currentVersion.lines = [];
        } else {
            if (line.startsWith("# ")) {
                line = `**${line.replace("# ", "")}**`;
            }
            
            currentVersion.lines.push(line);
        }
    }

    if (currentVersion) {
        versions.push(currentVersion);
    }

    return finalizeVersions(versions);
};

module.exports = parseChangelog;