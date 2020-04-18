import createInstance from "@/axios/axiosInstanceFactory";

export function getRequests() {
    return new Promise((resolve, reject) => createInstance()
        .get("ph-api/requests")
        .then(response => resolve(response.data))
        .catch(error => reject(error)));
}

export function clearRequests() {
    return new Promise((resolve, reject) =>
        createInstance()
            .delete("ph-api/requests")
            .then(() => resolve())
            .catch(error => reject(error)));
}