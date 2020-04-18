import {setDarkThemeEnabled} from "@/utils/sessionUtil";

export function storeDarkTheme(state, darkTheme) {
    state.settings.darkTheme = darkTheme;
    setDarkThemeEnabled(darkTheme);
}