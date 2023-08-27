export function getRootUrl(): string {
  return (window as any).rootUrl ?? "";
}

export function getUiRootUrl(): string {
  return `${getRootUrl()}/ph-ui`;
}
