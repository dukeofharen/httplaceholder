import Vue from "vue";
import Vuex from "vuex";
import {constructStore} from "@/store/storeConstructor";

Vue.use(Vuex);

const state = {
    userToken: "",
    settings: {
        darkTheme: false
    }
};

export default new Vuex.Store(constructStore(state));
