const themeType = {
    dark: "dark",
    light: "light"
};

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

window.addEventListener('DOMContentLoaded', function () {
    const theme = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches ? themeType.dark : themeType.light;
    switchTheme(theme);
    
    const themeSwitcherButton = document.getElementById("theme-switcher");
    themeSwitcherButton.addEventListener("click", function() {
        switchTheme();
    })
});