import Vue from 'vue';
import Vuex from 'vuex';
import actions from './actions';
import mutations from './mutations';
import getters from './getters';
import { authenticateResults, themes } from '@/constants';

Vue.use(Vuex);

export default new Vuex.Store({
    state: {
        metadata: {
            version: ''
        },
        authenticated: null,
        authenticationRequired: true,
        lastAuthenticateResult: authenticateResults.NOT_SET,
        userToken: "",
        requests: [],
        stubs: [],
        stubsDownloadString: '',
        toast: {
            message: "",
            type: "",
            timestamp: new Date().getTime()
        },
        lastSelectedStub: {
            id: ""
        },
        settings: {
            theme: themes.lightTheme
        }
    },
    mutations,
    getters,
    actions
});