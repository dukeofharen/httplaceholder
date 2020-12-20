import Vue from "vue";
import Vuex from "vuex";
import { getUserToken } from "@/utils/sessionUtil";

import metadata from "@/store/modules/metadata";

Vue.use(Vuex);

const token = getUserToken();
const state = {
  userToken: token || "",
  settings: {
    darkTheme: false
  },
  metadata: null
};

export default new Vuex.Store({
  modules: {
    metadata
  }
});
