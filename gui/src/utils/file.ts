export function getExtension(filename: string): string {
  const parts = filename.split('.')
  return parts[parts.length - 1].toLowerCase()
}
