import {parseKeyValue} from "@/utils/stubFormUtil";

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
