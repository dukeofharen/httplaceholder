import createInstance from "@/axios/axiosInstanceFactory";
import { mutationNames } from "@/store/storeConstants";

export function getMetadata(store) {
  return new Promise((resolve, reject) =>
    createInstance()
      .get("ph-api/metadata")
      .then(response => {
        resolve(response.data);
        store.commit(mutationNames.storeMetadata, response.data);
      })
      .catch(error => reject(error))
  );
}
