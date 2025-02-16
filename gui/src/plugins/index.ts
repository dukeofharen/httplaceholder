import './api'
import './bootstrap'
import './dayjs'
import './highlight'
import './roboto'
import './toastr'
import translatePlugin from './translate'
import sprintfPlugin from './sprintf'
import type { App } from 'vue'
import { createPinia } from 'pinia'
import router from '@/router'

export default function installPlugins(app: App<Element>) {
  app.use(createPinia())
  app.use(router)
  app.use(translatePlugin)
  app.use(sprintfPlugin)
}
