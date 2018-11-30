import Vue from 'vue';
import Vuex from 'vuex';

Vue.use(Vuex);

export default new Vuex.Store({
    state: {
        bla: 'ducoducoduco'
    },
    mutations: {

    },
    getters: {
        getBla: state => state.bla
    }
});