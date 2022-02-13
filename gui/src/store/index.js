import { createStore } from "vuex";
import general from "@/store/modules/general";
import requests from "@/store/modules/requests";
import stubForm from "@/store/modules/stubForm";
import stubs from "@/store/modules/stubs";
import tenants from "@/store/modules/tenants";

export default createStore({
  modules: {
    general,
    requests,
    stubForm,
    stubs,
    tenants,
  },
});
