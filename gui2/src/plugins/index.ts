import './dayjs'
import { createPinia } from 'pinia'
import router from '@/router'
import translatePlugin from "./translate";
import sprintfPlugin from "./sprintf";
import type { App } from 'vue'

export default function installPlugins(app: App<Element>) {
  app.use(createPinia());
  app.use(router);
  app.use(translatePlugin);
  app.use(sprintfPlugin);
}
