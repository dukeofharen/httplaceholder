import { translate } from '@/utils/translate'

export interface StringCheckingKeyword {
  key: string
  name: string
  description: string
}

export const keywords = {
  equals: 'equals',
  equalsci: 'equalsci',
  notequals: 'notequals',
  notequalsci: 'notequalsci',
  contains: 'contains',
  containsci: 'containsci',
  notcontains: 'notcontains',
  notcontainsci: 'notcontainsci',
  startswith: 'startswith',
  startswithci: 'startswithci',
  doesnotstartwith: 'doesnotstartwith',
  doesnotstartwithci: 'doesnotstartwithci',
  endswith: 'endswith',
  endswithci: 'endswithci',
  doesnotendwith: 'doesnotendwith',
  doesnotendwithci: 'doesnotendwithci',
  regex: 'regex',
  regexnomatches: 'regexnomatches',
  minlength: 'minlength',
  maxlength: 'maxlength',
  exactlength: 'exactlength',
}

export function getStringCheckingKeywords(): StringCheckingKeyword[] {
  return Object.keys(keywords).map((k) => {
    return {
      key: k,
      name: translate(`stringChecking.${k}`),
      description: translate(`stringChecking.${k}Description`),
    }
  }) as StringCheckingKeyword[]
}
