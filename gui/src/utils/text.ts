export function fromBase64(input: string): string | undefined {
  try {
    return window.atob(input);
  } catch (e) {
    console.log(e);
    return undefined;
  }
}

export function toBase64(input: string): string | undefined {
  try {
    return window.btoa(input);
  } catch (e) {
    console.log(e);
    return undefined;
  }
}
