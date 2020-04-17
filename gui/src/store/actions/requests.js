import createInstance from "@/axios/axiosInstanceFactory";
import {mutationNames} from "@/store/storeConstants";
import {resources} from "@/shared/resources";
import {toastSuccess} from "@/utils/toastUtil";

export function getRequests({commit}) {
    createInstance()
        .get("ph-api/requests")
        .then(response => commit(mutationNames.storeRequestsMutation, response.data));
}

export function clearRequests({commit}) {
    createInstance()
        .delete("ph-api/requests")
        .then(() => {
            toastSuccess(resources.requestsDeletedSuccessfully)
            commit(mutationNames.storeRequests, []);
        })
}