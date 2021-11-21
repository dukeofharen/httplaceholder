import { createStore } from "vuex";
import general from "@/store/modules/general";
import metadata from "@/store/modules/metadata";
import requests from "@/store/modules/requests";
import scenarios from "@/store/modules/scenarios";
import stubForm from "@/store/modules/stubForm";
import stubs from "@/store/modules/stubs";
import tenants from "@/store/modules/tenants";
import users from "@/store/modules/users";

export default createStore({
  modules: {
    general,
    metadata,
    requests,
    scenarios,
    stubForm,
    stubs,
    tenants,
    users,
  },
});
