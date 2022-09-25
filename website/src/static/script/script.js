const themeType = {
    dark: "dark",
    light: "light"
};

function fallbackCopyTextToClipboard(text) {
    const textArea = document.createElement("textarea");
    textArea.value = text;

    // Avoid scrolling to bottom
    textArea.style.top = "0";
    textArea.style.left = "0";
    textArea.style.position = "fixed";

    document.body.appendChild(textArea);
    textArea.focus();
    textArea.select();

    document.execCommand("copy");
    document.body.removeChild(textArea);
}

function copyTextToClipboard(text) {
    if (!navigator.clipboard) {
        return new Promise((resolve, reject) => {
            try {
                fallbackCopyTextToClipboard(text);
                resolve();
            } catch (e) {
                reject(e);
            }
        });
    }

    return navigator.clipboard.writeText(text);
}

function switchTheme(theme) {
    const html = document.getElementsByTagName("html")[0];
    const currentTheme = html.getAttribute("data-theme");
    let newTheme;
    if (theme) {
        newTheme = theme === themeType.dark ? themeType.dark : themeType.light;
    } else {
        newTheme = currentTheme === themeType.dark ? themeType.light : themeType.dark;
    }

    html.setAttribute("data-theme", newTheme);

    const themeSwitcherIcons = document.getElementsByClassName("theme-switch");
    for (let i = 0, max = themeSwitcherIcons.length; i < max; i++) {
        themeSwitcherIcons[i].style.display = "none";
    }

    const themeIconCssSelector = newTheme === themeType.dark ? "sun" : "moon";
    const themeIconElement = document.getElementsByClassName(themeIconCssSelector)[0];
    themeIconElement.style.display = "inline";
}

function onCopyLinkClick(e) {
    const target = e.target;
    const parent = target.parentNode;
    const code = parent.getElementsByTagName("code")[0];
    copyTextToClipboard(code.innerText).then(() => console.log("Copied to clipboard."));
}

function setupCopyableScripts() {
    const copyLinks = document.getElementsByClassName("copy-icon");
    for (let i = 0, max = copyLinks.length; i < max; i++) {
        copyLinks[i].addEventListener("click", onCopyLinkClick);
    }
}

window.addEventListener('DOMContentLoaded', function () {
    const theme = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches ? themeType.dark : themeType.light;
    switchTheme(theme);

    const themeSwitcherButton = document.getElementById("theme-switcher");
    themeSwitcherButton.addEventListener("click", function () {
        switchTheme();
    });

    setupCopyableScripts();
});