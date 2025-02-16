import { createApp } from 'vue'
import installPlugins from "@/plugins";
import { registerGlobalComponents } from "@/plugins/global-components";
import "@/style/style.scss";

import App from './App.vue'

const app = createApp(App)

registerGlobalComponents(app);
installPlugins(app);
app.mount('#app')
