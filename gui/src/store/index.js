import Vue from "vue";
import Vuex from "vuex";
import {authenticateResults} from "@/shared/constants";
import {constructStore} from "@/store/storeConstructor";

Vue.use(Vuex);

const state = {
    metadata: {
        version: ""
    },
    authenticated: null,
    authenticationRequired: true,
    lastAuthenticateResult: authenticateResults.NOT_SET,
    userToken: "",
    requests: [],
    stubs: [],
    tenantNames: [],
    stubsDownloadString: "",
    toast: {
        message: "",
        type: "",
        timestamp: new Date().getTime()
    },
    lastSelectedStub: {
        id: ""
    },
    settings: {
        darkTheme: false
    }
};

export default new Vuex.Store(constructStore(state));
