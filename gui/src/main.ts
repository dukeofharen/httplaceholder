import { createApp } from "vue";
import installPlugins from "@/plugins";
import { registerGlobalComponents } from "@/plugins/global-components";
import App from "./App.vue";
import "./style/style.scss";

const app = createApp(App);

registerGlobalComponents(app);
installPlugins(app);
app.mount("#app");
