export enum ResponseBodyType {
  text = "Text",
  json = "JSON",
  xml = "XML",
  html = "HTML",
  base64 = "Base64",
}

export function getValues(): string[] {
  return [
    ResponseBodyType.text,
    ResponseBodyType.json,
    ResponseBodyType.xml,
    ResponseBodyType.html,
    ResponseBodyType.base64,
  ];
}
