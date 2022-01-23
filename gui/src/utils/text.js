export function fromBase64(input) {
  try {
    return window.atob(input);
  } catch (e) {
    console.log(e);
    return null;
  }
}

export function toBase64(input) {
  try {
    return window.btoa(input);
  } catch (e) {
    console.log(e);
    return null;
  }
}
