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

// Source: https://stackoverflow.com/questions/16245767/creating-a-blob-from-a-base64-string-in-javascript
export function base64ToBlob(input: string): Blob {
  const sliceSize = 512;
  const byteCharacters = window.atob(input);
  const byteArrays = [];

  for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
    const slice = byteCharacters.slice(offset, offset + sliceSize);

    const byteNumbers = new Array(slice.length);
    for (let i = 0; i < slice.length; i++) {
      byteNumbers[i] = slice.charCodeAt(i);
    }

    const byteArray = new Uint8Array(byteNumbers);
    byteArrays.push(byteArray);
  }

  return new Blob(byteArrays);
}

// Source: https://stackoverflow.com/questions/8488729/how-to-count-the-number-of-lines-of-a-string-in-javascript
export function countNewlineCharacters(input: string): number {
  return (input.match(/\n/g) || "").length + 1;
}
