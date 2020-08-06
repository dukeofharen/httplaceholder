import Vue from "vue";
import Vuex from "vuex";
import {constructStore} from "@/store/storeConstructor";
import {getUserToken} from "@/utils/sessionUtil";
import {isHttpsValues, responseBodyTypes} from "@/shared/stubFormResources";
import addWatches from "@/store/watches";

Vue.use(Vuex);

const token = getUserToken();
const state = {
  userToken: token || "",
  settings: {
    darkTheme: false
  },
  metadata: null,
  stubForm: {
    queryStrings: "",
    body: "",
    formBody: "",
    xpath: "",
    xpathNamespaces: "",
    jsonPath: "",
    headers: "",
    isHttps: isHttpsValues.httpAndHttps,
    bodyResponseType: responseBodyTypes.text,
    responseBody: null,
    responseHeaders: "",
    stub: {
      id: null,
      tenant: null,
      description: null,
      priority: 0,
      conditions: {
        method: null,
        url: {
          path: null,
          query: null,
          fullPath: null,
          isHttps: null
        },
        body: null,
        form: null,
        xpath: null,
        jsonPath: null,
        basicAuthentication: {
          username: null,
          password: null
        },
        clientIp: null,
        hostname: null,
        headers: null
      },
      response: {
        statusCode: null,
        text: null,
        json: null,
        html: null,
        xml: null,
        base64: null,
        headers: null,
        extraDuration: null,
        temporaryRedirect: null,
        permanentRedirect: null,
        enableDynamicMode: false
      }
    }
  }
};

const store = new Vuex.Store(constructStore(state));
addWatches(store);
export default store;
