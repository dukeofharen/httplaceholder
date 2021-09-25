import { createStore } from "vuex";
import requests from "@/store/modules/requests";
import stubs from "@/store/modules/stubs";
import tenants from "@/store/modules/tenants";

export default createStore({
  modules: { requests, stubs, tenants },
});
