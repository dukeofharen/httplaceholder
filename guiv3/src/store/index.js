import { createStore } from "vuex";
import requests from "@/store/modules/requests";
import tenants from "@/store/modules/tenants";

export default createStore({
  modules: { requests, tenants },
});
