import createInstance from "@/axios/axiosInstanceFactory";

export function getMetadata({commit}) {
    return new Promise((resolve, reject) =>
        createInstance()
            .get("ph-api/metadata")
            .then(response => resolve(response.data))
            .catch(error => reject(error)));
}