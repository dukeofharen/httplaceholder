const rl = require("readline");

const readline = async (question) => {
    return new Promise((resolve, reject) => {
        const rlInterface = rl.createInterface({
            input: process.stdin,
            output: process.stdout
        });
        rlInterface.question(question, (answer) => {
            rlInterface.close();
            resolve(answer);
        });
    });
}

module.exports = {readline}