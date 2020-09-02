/* eslint-disable no-empty-pattern */
import createInstance from "@/axios/axiosInstanceFactory";

export function getRequestsOverview() {
  return new Promise((resolve, reject) =>
    createInstance()
      .get("ph-api/requests/overview")
      .then(response => resolve(response.data))
      .catch(error => reject(error))
  );
}

export function getRequest({}, correlationId) {
  return new Promise((resolve, reject) =>
    createInstance()
      .get(`ph-api/requests/${correlationId}`)
      .then(response => resolve(response.data))
      .catch(error => reject(error))
  );
}

export function clearRequests() {
  return new Promise((resolve, reject) =>
    createInstance()
      .delete("ph-api/requests")
      .then(() => resolve())
      .catch(error => reject(error))
  );
}
