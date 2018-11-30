import Vue from 'vue';
import Vuex from 'vuex';
import actions from './actions';
import mutations from './mutations';
import getters from './getters';
import { authenticateResults } from '@/constants';

Vue.use(Vuex);

export default new Vuex.Store({
    state: {
        metadata: {
            version: ''
        },
        authenticated: false,
        lastAuthenticateResult: authenticateResults.NOT_SET,
        userToken: ""
    },
    mutations,
    getters,
    actions
});