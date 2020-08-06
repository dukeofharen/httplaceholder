import {mutationNames} from "@/store/storeConstants";

export default function addWatches(store) {
  store.watch(state => state.stubForm.queryStrings, () => store.commit(mutationNames.storeStubQueryStrings));

  store.watch(state => state.stubForm.stub.conditions.url.query, () => store.commit(mutationNames.storeQueryStrings));

  store.watch(state => state.stubForm.isHttps, () => store.commit(mutationNames.storeStubIsHttpsSelected));

  store.watch(state => state.stubForm.stub.conditions.url.isHttps, () => store.commit(mutationNames.storeIsHttpsSelected));

  store.watch(state => state.stubForm.headers, () => store.commit(mutationNames.storeStubHeaders));

  store.watch(state => state.stubForm.stub.conditions.headers, () => store.commit(mutationNames.storeHeaders));

  store.watch(state => state.stubForm.body, () => store.commit(mutationNames.storeStubBody));

  store.watch(state => state.stubForm.stub.conditions.body, () => store.commit(mutationNames.storeBody));

  store.watch(state => state.stubForm.formBody, () => store.commit(mutationNames.storeStubFormBody));

  store.watch(state => state.stubForm.stub.conditions.form, () => store.commit(mutationNames.storeFormBody));

  store.watch(state => state.stubForm.xpath, () => store.commit(mutationNames.storeStubXPathAndNamespaces));

  store.watch(state => state.stubForm.xpathNamespaces, () => store.commit(mutationNames.storeStubXPathAndNamespaces));

  store.watch(state => state.stubForm.stub.conditions.xpath, () => store.commit(mutationNames.storeXPathAndNamespaces));

  store.watch(state => state.stubForm.jsonPath, () => store.commit(mutationNames.storeStubJsonPath));

  store.watch(state => state.stubForm.stub.conditions.jsonPath, () => store.commit(mutationNames.storeJsonPath));

  store.watch(state => state.stubForm.bodyResponseType, () => store.commit(mutationNames.storeResponseBodyType));
  store.watch(state => state.stubForm.responseBody, () => store.commit(mutationNames.storeResponseBodyType));

  store.watch(state => state.stubForm.stub.response.base64, () => store.commit(mutationNames.storeStubResponseBodyType));
  store.watch(state => state.stubForm.stub.response.text, () => store.commit(mutationNames.storeStubResponseBodyType));
  store.watch(state => state.stubForm.stub.response.html, () => store.commit(mutationNames.storeStubResponseBodyType));
  store.watch(state => state.stubForm.stub.response.json, () => store.commit(mutationNames.storeStubResponseBodyType));
  store.watch(state => state.stubForm.stub.response.xml, () => store.commit(mutationNames.storeStubResponseBodyType));

  store.watch(state => state.stubForm.responseHeaders, () => store.commit(mutationNames.storeStubResponseHeaders));

  store.watch(state => state.stubForm.stub.response.headers, () => store.commit(mutationNames.storeResponseHeaders));
}
