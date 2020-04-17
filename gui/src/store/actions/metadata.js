import createInstance from "@/axios/axiosInstanceFactory";

export function refreshMetadata({commit}) {
    const instance = createInstance();
    instance.get("ph-api/metadata").then(response => commit("storeMetadata", response.data));
}