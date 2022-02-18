import { createApp } from "vue";
import { createPinia } from "pinia";
import "@/plugins";
import registerGlobalComponents from "@/plugins/global-components";
import App from "./App.vue";
import router from "./router";
import "@/registerServiceWorker";

const app = createApp(App);

app.use(createPinia());
app.use(router);
registerGlobalComponents(app);
app.mount("#app");
