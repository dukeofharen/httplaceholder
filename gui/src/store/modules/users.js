import createInstance from "@/axios/axiosInstanceFactory";
import {
  clearUserToken,
  getUserToken,
  saveUserToken
} from "@/utils/sessionUtil";

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
  },
  storeAuthRequired(state, authRequired) {
    state.authRequired = authRequired;
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
