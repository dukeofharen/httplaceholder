import { createApp } from "vue";
import App from "@/App.vue";
import "@/registerServiceWorker";
import router from "@/router";
import store from "@/store";
import { createPinia } from "pinia";
import "@/plugins";
import registerGlobalComponents from "@/plugins/global-components";

const vueApp = createApp(App).use(store).use(router).use(createPinia());
registerGlobalComponents(vueApp);
vueApp.mount("#app");
