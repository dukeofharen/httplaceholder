export function storeToast(state, toast) {
    toast.timestamp = new Date().getTime();
    state.toast = toast;
}

export function storeDarkTheme(state, darkTheme) {
    state.settings.darkTheme = darkTheme;
}