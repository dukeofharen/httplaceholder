import createInstance from "@/axios/axiosInstanceFactory";

export function getTenantNames() {
    return new Promise((resolve, reject) => createInstance()
        .get("ph-api/tenants")
        .then(response => resolve(response.data))
        .catch(error => reject(error)));
}