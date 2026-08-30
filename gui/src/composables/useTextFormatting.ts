import xmlFormatter from 'xml-formatter'

export function useTextFormatting() {
  function xmlFormat(text: string) {
    return xmlFormatter(text)
  }

  function formFormat(form: string): string {
    let result = ''
    const formParts = form.split('&')
    for (const formPart of formParts) {
      const parts = formPart.split('=')
      if (parts.length > 1) {
        result += decodeURIComponent(parts[0]) + '=' + decodeURIComponent(parts[1]) + '\n'
      } else {
        result += formPart
      }
    }

    return result
  }

  function jsonFormat(text: string, spaces = 2) {
    try {
      const json = JSON.parse(text)
      return JSON.stringify(json, null, spaces)
    } catch {
      return ''
    }
  }

  return {xmlFormat, formFormat, jsonFormat}
}
