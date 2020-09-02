export function parseKeyValue(input) {
  let result = {};
  if (!input) {
    return result;
  }

  const lines = input.split(/\r?\n/);

  for (let line of lines) {
    let parts = line.split(":");
    if (parts.length <= 1) {
      continue;
    }

    let key = parts[0];
    result[key] = parts
      .slice(1)
      .join(":")
      .trim();
  }

  return result;
}

export function parseLines(input) {
  const result = input.split(/\r?\n/);
  if (result.every(l => !l)) {
    return [];
  }

  return result;
}
