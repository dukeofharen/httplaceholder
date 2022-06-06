export interface StringCheckingKeyword {
  key: string;
  name: string;
  description: string;
}

export function getStringCheckingKeywords(
  insertPresent: boolean
): StringCheckingKeyword[] {
  const result = [
    {
      key: "equals",
      name: "Equals",
      description:
        "checks if the input is exactly equal to this string, case sensitive",
    },
    {
      key: "equalsci",
      name: "Equals case insensitive",
      description: "same as keyword above, but case insensitive",
    },
    {
      key: "notequals",
      name: "Not equals",
      description:
        "checks if the input is not equal to this string, case sensitive",
    },
    {
      key: "notequalsci",
      name: "Not equals case insensitive",
      description: "same as keyword above, but case insensitive",
    },
    {
      key: "contains",
      name: "Contains",
      description: "checks if the input contains this string, case sensitive",
    },
    {
      key: "containsci",
      name: "Contains case insensitive",
      description: "same as keyword above, but case insensitive",
    },
    {
      key: "notcontains",
      name: "Not contains",
      description:
        "checks if the input does not contain this string, case sensitive",
    },
    {
      key: "notcontainsci",
      name: "Not contains case insensitive",
      description: "same as keyword above, but case insensitive",
    },
    {
      key: "startswith",
      name: "Starts with",
      description:
        "checks if the input starts with this string, case sensitive",
    },
    {
      key: "startswithci",
      name: "Starts with case insensitive",
      description: "same as keyword above, but case insensitive",
    },
    {
      key: "doesnotstartwith",
      name: "Does not start with",
      description:
        "checks if the input does not start with this string, case sensitive",
    },
    {
      key: "doesnotstartwithci",
      name: "Does not start with case insensitive",
      description: "same as keyword above, but case insensitive",
    },
    {
      key: "endswith",
      name: "Ends with",
      description: "checks if the input ends with this string, case sensitive",
    },
    {
      key: "endswithci",
      name: "Ends with case insensitive",
      description: "same as keyword above, but case insensitive",
    },
    {
      key: "doesnotendwith",
      name: "Does not end with",
      description:
        "checks if the input does not end with this string, case sensitive",
    },
    {
      key: "doesnotendwithci",
      name: "Does not end with case insensitive",
      description: "same as keyword above, but case insensitive",
    },
    {
      key: "regex",
      name: "Regular expression",
      description: "checks if the input matches this regular expression",
    },
    {
      key: "regexnomatches",
      name: "Regular expression no matches",
      description: "checks if the input does not match this regular expression",
    },
  ];
  if (insertPresent) {
    result.push({
      key: "present",
      name: "Present check",
      description:
        "checks if the given key can be found (does not check the value)",
    });
  }
  return result;
}
