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
    localStorage.theme = newTheme;

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

function setupStubCallSelector() {
    const element = document.getElementById("stub-call-selector");

    function showStubCall(value) {
        const stubCalls = document.getElementsByClassName("call");
        for (let i = 0, max = stubCalls.length; i < max; i++) {
            const stubCall = stubCalls[i];
            stubCall.style.display = "none";
        }

        if (value) {
            const stubCall = document.getElementsByClassName("call-" + value)[0];
            stubCall.style.display = "block";
        }
    }

    element.addEventListener("change", function () {
        showStubCall(element.value);
    });

    showStubCall(element.value);
}

window.addEventListener('DOMContentLoaded', function () {
    let theme = localStorage.theme ? localStorage.theme : window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches ? themeType.dark : themeType.light;
    switchTheme(theme);

    const themeSwitcherButton = document.getElementById("theme-switcher");
    themeSwitcherButton.addEventListener("click", function () {
        switchTheme();
    });

    setupCopyableScripts();
    setupStubCallSelector();
    hljs.highlightAll();
});