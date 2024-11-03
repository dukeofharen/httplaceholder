import { translate } from "@/utils/translate";

export interface StringCheckingKeyword {
  key: string;
  name: string;
  description: string;
}

export const keywords = {
  equals: "equals",
  equalsci: "equalsci",
  notequals: "notequals",
  notequalsci: "notequalsci",
  contains: "contains",
  containsci: "containsci",
  notcontains: "notcontains",
  notcontainsci: "notcontainsci",
  startswith: "startswith",
  startswithci: "startswithci",
  doesnotstartwith: "doesnotstartwith",
  doesnotstartwithci: "doesnotstartwithci",
  endswith: "endswith",
  endswithci: "endswithci",
  doesnotendwith: "doesnotendwith",
  doesnotendwithci: "doesnotendwithci",
  regex: "regex",
  regexnomatches: "regexnomatches",
  minlength: "minlength",
  maxlength: "maxlength",
  exactlength: "exactlength",
};

export function getStringCheckingKeywords(): StringCheckingKeyword[] {
  const result = [
    {
      key: keywords.equals,
      name: translate("stringChecking.equals"),
      description: translate("stringChecking.equalsDescription"),
    },
    {
      key: keywords.equalsci,
      name: translate("stringChecking.equalsci"),
      description: translate("stringChecking.equalsciDescription"),
    },
    {
      key: keywords.notequals,
      name: translate("stringChecking.notequals"),
      description: translate("stringChecking.notequalsDescription"),
    },
    {
      key: keywords.notequalsci,
      name: translate("stringChecking.notequalsci"),
      description: translate("stringChecking.notequalsciDescription"),
    },
    {
      key: keywords.contains,
      name: translate("stringChecking.contains"),
      description: translate("stringChecking.containsDescription"),
    },
    {
      key: keywords.containsci,
      name: translate("stringChecking.containsci"),
      description: translate("stringChecking.containsciDescription"),
    },
    {
      key: keywords.notcontains,
      name: translate("stringChecking.notcontains"),
      description: translate("stringChecking.notcontainsDescription"),
    },
    {
      key: keywords.notcontainsci,
      name: translate("stringChecking.notcontainsci"),
      description: translate("stringChecking.notcontainsciDescription"),
    },
    {
      key: keywords.startswith,
      name: translate("stringChecking.startswith"),
      description: translate("stringChecking.startswithDescription"),
    },
    {
      key: keywords.startswithci,
      name: translate("stringChecking.startswithci"),
      description: translate("stringChecking.startswithciDescription"),
    },
    {
      key: keywords.doesnotstartwith,
      name: translate("stringChecking.doesnotstartwith"),
      description: translate("stringChecking.doesnotstartwithDescription"),
    },
    {
      key: keywords.doesnotstartwithci,
      name: translate("stringChecking.doesnotstartwithci"),
      description: translate("stringChecking.doesnotstartwithciDescription"),
    },
    {
      key: keywords.endswith,
      name: translate("stringChecking.endswith"),
      description: translate("stringChecking.endswithciDescription"),
    },
    {
      key: keywords.endswithci,
      name: translate("stringChecking.endswithci"),
      description: translate("stringChecking.endswithDescription"),
    },
    {
      key: keywords.doesnotendwith,
      name: translate("stringChecking.doesnotendwith"),
      description: translate("stringChecking.doesnotendwithDescription"),
    },
    {
      key: keywords.doesnotendwithci,
      name: translate("stringChecking.doesnotendwithci"),
      description: translate("stringChecking.doesnotendwithciDescription"),
    },
    {
      key: keywords.regex,
      name: translate("stringChecking.regex"),
      description: translate("stringChecking.regexDescription"),
    },
    {
      key: keywords.regexnomatches,
      name: translate("stringChecking.regexnomatches"),
      description: translate("stringChecking.regexnomatchesDescription"),
    },
    {
      key: keywords.minlength,
      name: translate("stringChecking.minlength"),
      description: translate("stringChecking.minLengthDescription"),
    },
    {
      key: keywords.maxlength,
      name: translate("stringChecking.maxlength"),
      description: translate("stringChecking.maxlengthDescription"),
    },
    {
      key: keywords.exactlength,
      name: translate("stringChecking.exactlength"),
      description: translate("stringChecking.exactlengthDescription"),
    },
  ];
  return result;
}
