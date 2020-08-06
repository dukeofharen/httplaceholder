import {mutationNames} from "@/store/storeConstants";

export default function addWatches(store) {
  store.watch(state => state.stubForm.queryStrings, () => {
    store.commit(mutationNames.storeStubQueryStrings);
  });

  store.watch(state => state.stubForm.stub.conditions.url.query, value => {


    store.commit(mutationNames.storeQueryStrings)
  });
}
