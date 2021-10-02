import { createStore } from "vuex";
import metadata from "@/store/modules/metadata";
import requests from "@/store/modules/requests";
import stubForm from "@/store/modules/stubForm";
import stubs from "@/store/modules/stubs";
import tenants from "@/store/modules/tenants";

export default createStore({
  modules: { metadata, requests, stubForm, stubs, tenants },
});
