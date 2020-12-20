import Vue from "vue";
import Vuex from "vuex";

import general from "@/store/modules/general";
import metadata from "@/store/modules/metadata";
import requests from "@/store/modules/requests";
import stubForm from "@/store/modules/stubForm";
import stubs from "@/store/modules/stubs";
import tenants from "@/store/modules/tenants";
import users from "@/store/modules/users";

Vue.use(Vuex);

export default new Vuex.Store({
  modules: {
    metadata,
    requests,
    stubForm,
    stubs,
    tenants,
    users,
    general
  }
});
