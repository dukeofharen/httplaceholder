import { parseKeyValue, parseLines } from "@/utils/stubFormUtil";
import { isHttpsValues, responseBodyTypes } from "@/shared/stubFormResources";
import { getEmptyStubForm } from "@/store/storeConstants";

export function storeStubQueryStrings(state) {
  const result = parseKeyValue(state.stubForm.queryStrings);
  if (!Object.keys(result).length) {
    state.stubForm.stub.conditions.url.query = null;
  } else {
    state.stubForm.stub.conditions.url.query = result;
  }
}

export function storeQueryStrings(state) {
  let result = "";
  const value = state.stubForm.stub.conditions.url.query;
  if (value) {
    const keys = Object.keys(value);
    if (keys.length) {
      const list = [];
      for (let key of keys) {
        list.push(`${key}: ${value[key]}`);
      }

      result = list.join("\n");
    }
  }

  state.stubForm.queryStrings = result;
}

export function storeStubIsHttpsSelected(state) {
  // I had to add this intermediate function, because for some reason, Vuetify doesn't allow the binding of a "null" value.
  switch (state.stubForm.isHttps) {
    case isHttpsValues.httpAndHttps:
      state.stubForm.stub.conditions.url.isHttps = null;
      break;
    case isHttpsValues.onlyHttp:
      state.stubForm.stub.conditions.url.isHttps = false;
      break;
    case isHttpsValues.onlyHttps:
      state.stubForm.stub.conditions.url.isHttps = true;
      break;
  }
}

export function storeIsHttpsSelected(state) {
  const isHttps = state.stubForm.stub.conditions.url.isHttps;
  let result;
  if (isHttps === true) {
    result = isHttpsValues.onlyHttps;
  } else if (isHttps === false) {
    result = isHttpsValues.onlyHttp;
  } else {
    result = isHttpsValues.httpAndHttps;
  }

  state.stubForm.isHttps = result;
}

export function storeStubHeaders(state) {
  const result = parseKeyValue(state.stubForm.headers);
  state.stubForm.stub.conditions.headers = Object.keys(result).length
    ? result
    : null;
}

export function storeHeaders(state) {
  let result = "";
  const value = state.stubForm.stub.conditions.headers;
  if (value) {
    const keys = Object.keys(value);
    if (keys.length) {
      const list = [];
      for (let key of keys) {
        list.push(`${key}: ${value[key]}`);
      }

      result = list.join("\n");
    }
  }

  state.stubForm.headers = result;
}

export function storeStubBody(state) {
  const result = parseLines(state.stubForm.body);
  state.stubForm.stub.conditions.body = result.length ? result : null;
}

export function storeBody(state) {
  const value = state.stubForm.stub.conditions.body;
  state.stubForm.body = value && value.length ? value.join("\n") : "";
}

export function storeStubFormBody(state) {
  const result = parseKeyValue(state.stubForm.formBody);
  const keys = Object.keys(result);
  if (!keys.length) {
    state.stubForm.stub.conditions.form = null;
  } else {
    state.stubForm.stub.conditions.form = keys.map(k => ({
      key: k,
      value: result[k]
    }));
  }
}

export function storeFormBody(state) {
  let result = "";
  const value = state.stubForm.stub.conditions.form;
  if (value && value.length) {
    const list = [];
    for (let line of value) {
      list.push(`${line.key}: ${line.value}`);
    }

    result = list.join("\n");
  }

  state.stubForm.formBody = result;
}

export function storeStubXPathAndNamespaces(state) {
  const result = parseLines(state.stubForm.xpath);
  if (!result.length) {
    state.stubForm.stub.conditions.xpath = null;
  } else {
    const expressions = result.map(e => ({ queryString: e }));
    const nsResult = parseKeyValue(state.stubForm.xpathNamespaces);
    const nsKeys = Object.keys(nsResult);
    let namespaces = {};
    if (nsKeys.length) {
      for (let key of nsKeys) {
        namespaces[key] = nsResult[key];
      }
    } else {
      namespaces = null;
    }

    for (let expression of expressions) {
      expression.namespaces = namespaces;
    }

    state.stubForm.stub.conditions.xpath = expressions;
  }
}

export function storeXPathAndNamespaces(state) {
  // Expressions
  if (state.stubForm.stub.conditions.xpath) {
    const value = state.stubForm.stub.conditions.xpath.map(x => x.queryString);
    state.stubForm.xpath = value && value.length ? value.join("\n") : "";

    // XML namespaces
    let nsResult = null;
    const namespaces = state.stubForm.stub.conditions.xpath.length
      ? state.stubForm.stub.conditions.xpath[0].namespaces
      : null;
    if (namespaces) {
      const list = [];
      const keys = Object.keys(namespaces);
      for (let key of keys) {
        list.push(`${key}: ${namespaces[key]}`);
      }

      nsResult = list.join("\n");
    }

    state.stubForm.xpathNamespaces = nsResult;
  } else {
    state.stubForm.xpath = "";
    state.stubForm.xpathNamespaces = "";
  }
}

export function storeStubJsonPath(state) {
  const result = parseLines(state.stubForm.jsonPath);
  state.stubForm.stub.conditions.jsonPath = result.length ? result : null;
}

export function storeJsonPath(state) {
  const value = state.stubForm.stub.conditions.jsonPath;
  state.stubForm.jsonPath = value && value.length ? value.join("\n") : "";
}

export function storeResponseBodyType(state) {
  state.stubForm.stub.response.html = null;
  state.stubForm.stub.response.text = null;
  state.stubForm.stub.response.json = null;
  state.stubForm.stub.response.xml = null;
  state.stubForm.stub.response.base64 = null;
  const body = state.stubForm.responseBody;
  switch (state.stubForm.bodyResponseType) {
    case responseBodyTypes.custom:
    case responseBodyTypes.text:
      state.stubForm.stub.response.text = body;
      break;
    case responseBodyTypes.html:
      state.stubForm.stub.response.html = body;
      break;
    case responseBodyTypes.json:
      state.stubForm.stub.response.json = body;
      break;
    case responseBodyTypes.xml:
      state.stubForm.stub.response.xml = body;
      break;
    case responseBodyTypes.base64:
      state.stubForm.stub.response.base64 = body;
      break;
  }
}

export function storeStubResponseBodyType(state) {
  const response = state.stubForm.stub.response;
  let result;
  if (response.text) {
    result = responseBodyTypes.text;
    state.stubForm.responseBody = response.text;
  } else if (response.html) {
    result = responseBodyTypes.html;
    state.stubForm.responseBody = response.html;
  } else if (response.json) {
    result = responseBodyTypes.json;
    state.stubForm.responseBody = response.json;
  } else if (response.xml) {
    result = responseBodyTypes.xml;
    state.stubForm.responseBody = response.xml;
  } else if (response.base64) {
    result = responseBodyTypes.base64;
    state.stubForm.responseBody = response.base64;
  } else {
    result = responseBodyTypes.empty;
    state.stubForm.responseBody = null;
  }

  state.stubForm.bodyResponseType = result;
}

export function storeStubResponseHeaders(state) {
  const result = parseKeyValue(state.stubForm.responseHeaders);
  state.stubForm.stub.response.headers = Object.keys(result).length
    ? result
    : null;
}

export function storeResponseHeaders(state) {
  let result = "";
  const value = state.stubForm.stub.response.headers;
  if (value) {
    const keys = Object.keys(value);
    if (keys.length) {
      const list = [];
      for (let key of keys) {
        list.push(`${key}: ${value[key]}`);
      }

      result = list.join("\n");
    }
  }

  state.stubForm.responseHeaders = result;
}

export function clearStubForm(state) {
  state.stubForm = getEmptyStubForm();
}

export function setResponseHeader(state, header) {
  const headers = state.stubForm.stub.response.headers || {};
  const keys = Object.keys(headers);
  const headerKey =
    keys.find(k => k.toLowerCase() === header.key.toLowerCase()) || header.key;
  headers[headerKey] = header.value;

  // We have to clone the object for the watchers to pick this change up.
  state.stubForm.stub.response.headers = JSON.parse(JSON.stringify(headers));
}
