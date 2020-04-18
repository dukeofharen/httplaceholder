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
    return new Promise((resolve, reject) =>
        getUser(username, password, commit)
            .then(() => {
                // No authentication on endpoint, so no login required.
                resolve(false);
            })
            .catch(error => {
                // Authentication required, so show login screen.
                if (error.response.status === 401) {
                    resolve(true);
                }
            }));
}

export function authenticate({commit}, payload) {
    return new Promise((resolve, reject) =>
        getUser(payload.username, payload.password, commit)
            .then(() => resolve())
            .catch(error => reject(error)));
}