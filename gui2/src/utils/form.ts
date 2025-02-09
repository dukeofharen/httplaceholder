export function formFormat(form: string): string {
  let result = "";
  const formParts = form.split("&");
  for (const formPart of formParts) {
    const parts = formPart.split("=");
    if (parts.length > 1) {
      result +=
        decodeURIComponent(parts[0]) +
        "=" +
        decodeURIComponent(parts[1]) +
        "\n";
    } else {
      result += formPart;
    }
  }

  return result;
}
