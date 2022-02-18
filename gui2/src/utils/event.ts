export function shouldSave(e: KeyboardEvent) {
  return e.ctrlKey && (e.key === "s" || e.key === "Enter");
}

export function escapePressed(e: KeyboardEvent) {
  return e.key === "Escape";
}
