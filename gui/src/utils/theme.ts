export function browserUsesDarkTheme(): boolean {
  return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches
}
