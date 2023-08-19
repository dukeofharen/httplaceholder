export function getRootUrl(): string {
  return (window as any).rootUrl ?? "";
}
