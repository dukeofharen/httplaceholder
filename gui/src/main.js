// General
import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import "./registerServiceWorker";

// Fonts
import "typeface-roboto";

// Overrides
import "./utils/stringFormat";

// Plugins
import "@/plugins/toastr";
import "@/plugins/filters";
import "@/plugins/codemirror";
import "@/plugins/shortkey";
import vuetify from "@/plugins/vuetify";

import store from "@/store";

Vue.config.productionTip = false;

new Vue({
  store,
  router,
  vuetify,
  render: h => h(App)
}).$mount("#app");
