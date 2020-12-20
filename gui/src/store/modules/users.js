import createInstance from "@/axios/axiosInstanceFactory";
import {clearUserToken, getUserToken, saveUserToken} from "@/utils/sessionUtil";

const token = getUserToken();
const getUser = (username, password, commit) => {
  const token = btoa(`${username}:${password}`);
  return createInstance()
    .get(`ph-api/users/${username}`, {
      headers: {
        Authorization: `Basic ${token}`
      }
    })
    .then(() => commit("storeUserToken", token));
};

const state = () => ({
  userToken: token || ""
});

const actions = {
  ensureAuthenticated({ commit }) {
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
          } else {
            reject(error);
          }
        })
    );
  },
  authenticate({ commit }, payload) {
    return new Promise((resolve, reject) =>
      getUser(payload.username, payload.password, commit)
        .then(() => resolve())
        .catch(error => reject(error))
    );
  }
};

const mutations = {
  storeUserToken(state, token) {
    state.userToken = token;
    if (!token) {
      clearUserToken();
    } else {
      saveUserToken(token);
    }
  }
};

const getters = {
  getAuthenticated(state) {
    return !!state.userToken;
  },
  getUserToken(state) {
    return state.userToken;
  }
};

export default {
  namespaced: true,
  state,
  getters,
  mutations,
  actions
};
