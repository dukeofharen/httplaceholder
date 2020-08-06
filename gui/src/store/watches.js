import {mutationNames} from "@/store/storeConstants";

export default function addWatches(store) {
  store.watch(state => state.stubForm.queryStrings, () => store.commit(mutationNames.storeStubQueryStrings));

  store.watch(state => state.stubForm.stub.conditions.url.query, () => store.commit(mutationNames.storeQueryStrings));

  store.watch(state => state.stubForm.isHttps, () => store.commit(mutationNames.storeStubIsHttpsSelected));

  store.watch(state => state.stubForm.stub.conditions.url.isHttps, () => store.commit(mutationNames.storeIsHttpsSelected));
}
