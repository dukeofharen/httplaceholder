import createInstance from "@/axios/axiosInstanceFactory";
import {mutationNames} from "@/store/storeConstants";

export function getTenantNames({commit, state}) {
    createInstance()
        .get("ph-api/tenants")
        .then(response => commit(mutationNames.storeTenantNames, response.data));
}