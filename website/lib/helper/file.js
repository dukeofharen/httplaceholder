const fsPromises = require("fs/promises");
const {copy, exists, ensureDir, remove} = require("fs-extra");

const readFile = async (filePath) => {
    return (await fsPromises.readFile(filePath)).toString();
};

module.exports = {
    readFile,
    copy,
    exists,
    ensureDir,
    remove,
    writeFile: fsPromises.writeFile,
    readdir: fsPromises.readdir
};