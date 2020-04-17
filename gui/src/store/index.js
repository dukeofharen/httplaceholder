import Vue from "vue";
import Vuex from "vuex";
import {constructStore} from "@/store/storeConstructor";

Vue.use(Vuex);

const state = {
    metadata: {
        version: ""
    },
    authenticated: null,
    authenticationRequired: true,
    userToken: "",
    lastSelectedStub: {
        id: ""
    },
    settings: {
        darkTheme: false
    }
};

export default new Vuex.Store(constructStore(state));
