export function formFormat(form) {
  let result = "";
  let formParts = form.split("&");
  for (let formPart of formParts) {
    let parts = formPart.split("=");
    result +=
      decodeURIComponent(parts[0]) + "=" + decodeURIComponent(parts[1]) + "\n";
  }

  return result;
}
