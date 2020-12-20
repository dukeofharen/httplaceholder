import Vue from "vue";
import Vuex from "vuex";
import { getUserToken } from "@/utils/sessionUtil";

import metadata from "@/store/modules/metadata";
import requests from "@/store/modules/requests";
import stubs from "@/store/modules/stubs";
import tenants from "@/store/modules/tenants";

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
    metadata,
    requests,
    stubs,
    tenants
  }
});
