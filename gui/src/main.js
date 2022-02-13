import { createApp } from "vue";
import App from "@/App.vue";
import "@/registerServiceWorker";
import router from "@/router";
import { createPinia } from "pinia";
import "@/plugins";
import registerGlobalComponents from "@/plugins/global-components";

const vueApp = createApp(App).use(router).use(createPinia());
registerGlobalComponents(vueApp);
vueApp.mount("#app");
