import {parseKeyValue, parseLines} from "@/utils/stubFormUtil";
import {isHttpsValues} from "@/shared/stubFormResources";

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
  const keys = Object.keys(value);
  if (keys.length) {
    const list = [];
    for (let key of keys) {
      list.push(`${key}: ${value[key]}`);
    }

    result = list.join("\n");
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
  state.stubForm.stub.conditions.headers = Object.keys(result).length ? result : null;
}

export function storeHeaders(state) {
  let result = "";
  const value = state.stubForm.stub.conditions.headers;
  const keys = Object.keys(value);
  if (keys.length) {
    const list = [];
    for (let key of keys) {
      list.push(`${key}: ${value[key]}`);
    }

    result = list.join("\n");
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
    state.stubForm.stub.conditions.form = keys.map(k => ({key: k, value: result[k]}));
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
    const expressions = result.map(e => ({queryString: e}));
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
  const value = state.stubForm.stub.conditions.xpath.map(x => x.queryString);
  state.stubForm.xpath = value && value.length ? value.join("\n") : "";

  // XML namespaces
  let nsResult = null;
  const namespaces = state.stubForm.stub.conditions.xpath.length ? state.stubForm.stub.conditions.xpath[0].namespaces : null;
  if (namespaces) {
    const list = [];
    const keys = Object.keys(namespaces);
    for (let key of keys) {
      list.push(`${key}: ${namespaces[key]}`);
    }

    nsResult = list.join("\n");
  }

  state.stubForm.xpathNamespaces = nsResult;
}
