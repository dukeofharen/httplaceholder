export function formFormat(form: string): string {
  let result = "";
  const formParts = form.split("&");
  for (const formPart of formParts) {
    const parts = formPart.split("=");
    result +=
      decodeURIComponent(parts[0]) + "=" + decodeURIComponent(parts[1]) + "\n";
  }

  return result;
}
