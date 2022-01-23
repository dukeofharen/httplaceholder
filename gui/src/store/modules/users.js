import { clearUserToken, getUserToken, saveUserToken } from "@/utils/session";
import { get } from "@/utils/api";
import { toBase64 } from "@/utils/text";

const token = getUserToken();
const getUser = (username, password, commit) => {
  const token = toBase64(`${username}:${password}`);
  return get(`/ph-api/users/${username}`, {
    headers: {
      Authorization: `Basic ${token}`,
    },
  }).then(() => commit("storeUserToken", token));
};

const state = () => ({
  userToken: token || "",
});

const actions = {
  authenticate({ commit }, payload) {
    return getUser(payload.username, payload.password, commit)
      .then((response) => Promise.resolve(response))
      .catch((error) => Promise.reject(error));
  },
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
  logOut(state) {
    clearUserToken();
    state.userToken = null;
    document.cookie = `HttPlaceholderLoggedin=;expires=${new Date(
      0
    ).toUTCString()}`;
  },
};

const getters = {
  getAuthenticated(state) {
    return !!state.userToken;
  },
  getUserToken(state) {
    return state.userToken;
  },
};

export default {
  namespaced: true,
  state,
  getters,
  mutations,
  actions,
};
