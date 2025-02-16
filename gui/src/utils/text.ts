export function fromBase64(input: string): string | undefined {
  // Source: https://stackoverflow.com/questions/30106476/using-javascripts-atob-to-decode-base64-doesnt-properly-decode-utf-8-strings
  return decodeURIComponent(
    window
      .atob(input)
      .split('')
      .map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)
      })
      .join(''),
  )
}

export function toBase64(input: string): string | undefined {
  try {
    return window.btoa(input)
  } catch (e) {
    console.log(e)
    return undefined
  }
}

// Source: https://stackoverflow.com/questions/16245767/creating-a-blob-from-a-base64-string-in-javascript
export function base64ToBlob(input: string): Blob {
  const sliceSize = 512
  const byteCharacters = window.atob(input)
  const byteArrays = []

  for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
    const slice = byteCharacters.slice(offset, offset + sliceSize)

    const byteNumbers = new Array(slice.length)
    for (let i = 0; i < slice.length; i++) {
      byteNumbers[i] = slice.charCodeAt(i)
    }

    const byteArray = new Uint8Array(byteNumbers)
    byteArrays.push(byteArray)
  }

  return new Blob(byteArrays)
}

// Source: https://stackoverflow.com/questions/8488729/how-to-count-the-number-of-lines-of-a-string-in-javascript
export function countNewlineCharacters(input: string): number {
  return (input.match(/\n/g) || '').length + 1
}
