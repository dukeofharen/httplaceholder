import { createStore } from "vuex";
import general from "@/store/modules/general";
import requests from "@/store/modules/requests";
import stubForm from "@/store/modules/stubForm";

export default createStore({
  modules: {
    general,
    requests,
    stubForm,
  },
});
