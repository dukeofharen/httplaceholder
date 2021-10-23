export function shouldSave(e) {
  return e.ctrlKey && (e.key === "s" || e.key === "Enter");
}
