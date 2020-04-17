// General
import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import "./registerServiceWorker";

// Fonts
import "typeface-roboto";

// Styles
import "@/css/style.css";

// VueShortkey
import VueShortKey from "vue-shortkey";

// CodeMirror
import VueCodemirror from "vue-codemirror";
import "codemirror/lib/codemirror.css";
import "codemirror/mode/yaml/yaml.js";
import "@/css/material-darker.css";

// Toastr
import "toastr/build/toastr.css";

// Filters
import "./filters/yaml";
import "./filters/datetime";

import "./functions/stringFormat";

import store from "./store/store.js";
import vuetify from "./plugins/vuetify";

Vue.config.productionTip = false;

Vue.use(VueCodemirror);
Vue.use(VueShortKey);

new Vue({
  store,
  router,
  vuetify,
  render: h => h(App)
}).$mount("#app");
