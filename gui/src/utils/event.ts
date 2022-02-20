export function shouldSave(e: KeyboardEvent): boolean {
  return e.ctrlKey && (e.key === "s" || e.key === "Enter");
}

export function escapePressed(e: KeyboardEvent): boolean {
  return e.key === "Escape";
}
