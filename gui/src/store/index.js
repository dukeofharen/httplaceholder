import { createStore } from "vuex";
import general from "@/store/modules/general";
import stubForm from "@/store/modules/stubForm";

export default createStore({
  modules: {
    general,
    stubForm,
  },
});
