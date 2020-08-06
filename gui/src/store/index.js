import Vue from "vue";
import Vuex from "vuex";
import { constructStore } from "@/store/storeConstructor";
import { getUserToken } from "@/utils/sessionUtil";

Vue.use(Vuex);

const token = getUserToken();
const state = {
  userToken: token || "",
  settings: {
    darkTheme: false
  },
  metadata: null
};

export default new Vuex.Store(constructStore(state));
