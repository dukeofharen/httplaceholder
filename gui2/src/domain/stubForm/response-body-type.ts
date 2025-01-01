export enum ResponseBodyType {
  text = 'Text',
  json = 'JSON',
  xml = 'XML',
  html = 'HTML',
  base64 = 'Base64',
}

export function getValues(): string[] {
  const enumType = ResponseBodyType as any
  return Object.keys(ResponseBodyType).map((key) => enumType[key])
}
