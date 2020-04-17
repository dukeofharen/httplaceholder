import createInstance from "@/axios/axiosInstanceFactory";
import {mutationNames} from "@/store/storeConstants";

const getUser = (username, password, commit) => {
    const token = btoa(`${username}:${password}`);
    return createInstance()
        .get(`ph-api/users/${username}`, {
            headers: {
                Authorization: `Basic ${token}`
            }
        })
        .then(() => commit(mutationNames.userTokenMutation, token));
};

export function ensureAuthenticated({commit}) {
    let username = "testUser";
    let password = "testPassword";
    getUser(username, password, commit)
        .then(() => {
            // No authentication on endpoint, so no login required.
            commit(mutationNames.storeAuthMutation, true);
            commit(mutationNames.storeAuthRequired, false);
        })
        .catch(error => {
            // Authentication required, so show login screen.
            if (error.response.status === 401) {
                commit(mutationNames.storeAuthMutation, false);
                commit(mutationNames.storeAuthRequired, true);
            }
        });
}

export function authenticate({commit}, payload) {
    return new Promise((resolve, reject) => getUser(payload.username, payload.password, commit)
        .then(() => {
            commit(mutationNames.storeAuthMutation, true);
            resolve();
        })
        .catch(error => reject(error)));
}