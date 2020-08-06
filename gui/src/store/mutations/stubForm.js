import {parseKeyValue} from "@/utils/stubFormUtil";
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
