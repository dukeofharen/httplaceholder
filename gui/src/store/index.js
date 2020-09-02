import Vue from "vue";
import Vuex from "vuex";
import { constructStore } from "@/store/storeConstructor";
import { getUserToken } from "@/utils/sessionUtil";

import addWatches from "@/store/watches";
import { getEmptyStubForm } from "@/store/storeConstants";

Vue.use(Vuex);

const token = getUserToken();
const state = {
  userToken: token || "",
  settings: {
    darkTheme: false
  },
  metadata: null,
  stubForm: getEmptyStubForm()
};

const store = new Vuex.Store(constructStore(state));
addWatches(store);
export default store;
