export function parseKeyValue(input) {
  let result = {};
  const lines = input.split(/\r?\n/);

  for (let line of lines) {
    let parts = line.split(":");
    if (parts.length <= 1) {
      continue;
    }

    let key = parts[0];
    let value = parts.slice(1).join(":").trim();
    result[key] = value;
  }

  return result;
}
