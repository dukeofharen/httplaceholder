const themeType = {
    dark: "dark",
    light: "light"
};

function switchTheme(theme) {
    const html = document.getElementsByTagName("html");
    const currentTheme = html[0].getAttribute("data-theme");
    let newTheme;
    if (theme) {
        newTheme = theme === themeType.dark ? themeType.dark : themeType.light;
    } else {
        newTheme = currentTheme === themeType.dark ? themeType.light : themeType.dark;
    }

    html[0].setAttribute("data-theme", newTheme);
}

window.addEventListener('DOMContentLoaded', function () {
    const theme = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches ? themeType.dark : themeType.light;
    switchTheme(theme);
});