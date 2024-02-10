export interface StringCheckingKeyword {
  key: string;
  name: string;
  description: string;
  defaultValue?: any;
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
  present: "present",
};

export function getStringCheckingKeywords(
  insertPresent: boolean,
): StringCheckingKeyword[] {
  const result = [
    {
      key: keywords.equals,
      name: "Equals",
      description:
        "checks if the input is exactly equal to this string, case sensitive",
    },
    {
      key: keywords.equalsci,
      name: "Equals case insensitive",
      description: "same as keyword above, but case insensitive",
    },
    {
      key: keywords.notequals,
      name: "Not equals",
      description:
        "checks if the input is not equal to this string, case sensitive",
    },
    {
      key: keywords.notequalsci,
      name: "Not equals case insensitive",
      description: "same as keyword above, but case insensitive",
    },
    {
      key: keywords.contains,
      name: "Contains",
      description: "checks if the input contains this string, case sensitive",
    },
    {
      key: keywords.containsci,
      name: "Contains case insensitive",
      description: "same as keyword above, but case insensitive",
    },
    {
      key: keywords.notcontains,
      name: "Not contains",
      description:
        "checks if the input does not contain this string, case sensitive",
    },
    {
      key: keywords.notcontainsci,
      name: "Not contains case insensitive",
      description: "same as keyword above, but case insensitive",
    },
    {
      key: keywords.startswith,
      name: "Starts with",
      description:
        "checks if the input starts with this string, case sensitive",
    },
    {
      key: keywords.startswithci,
      name: "Starts with case insensitive",
      description: "same as keyword above, but case insensitive",
    },
    {
      key: keywords.doesnotstartwith,
      name: "Does not start with",
      description:
        "checks if the input does not start with this string, case sensitive",
    },
    {
      key: keywords.doesnotstartwithci,
      name: "Does not start with case insensitive",
      description: "same as keyword above, but case insensitive",
    },
    {
      key: keywords.endswith,
      name: "Ends with",
      description: "checks if the input ends with this string, case sensitive",
    },
    {
      key: keywords.endswithci,
      name: "Ends with case insensitive",
      description: "same as keyword above, but case insensitive",
    },
    {
      key: keywords.doesnotendwith,
      name: "Does not end with",
      description:
        "checks if the input does not end with this string, case sensitive",
    },
    {
      key: keywords.doesnotendwithci,
      name: "Does not end with case insensitive",
      description: "same as keyword above, but case insensitive",
    },
    {
      key: keywords.regex,
      name: "Regular expression",
      description: "checks if the input matches this regular expression",
    },
    {
      key: keywords.regexnomatches,
      name: "Regular expression no matches",
      description: "checks if the input does not match this regular expression",
    },
    {
      key: keywords.minlength,
      name: "Minimum length",
      description: "checks if the input has a minimum (inclusive) length",
      defaultValue: 10,
    },
    {
      key: keywords.maxlength,
      name: "Maximum length",
      description: "checks if the input has a maximum (inclusive) length",
      defaultValue: 10,
    },
    {
      key: keywords.exactlength,
      name: "Exact length",
      description: "checks if the input has an exact length",
      defaultValue: 10,
    },
  ];
  if (insertPresent) {
    result.push({
      key: keywords.present,
      name: "Present check",
      description:
        "checks if the given key can be found (does not check the value)",
    });
  }
  return result;
}
