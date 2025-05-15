import { createApp } from 'vue'
import installPlugins from '@/plugins'
import '@/style/style.scss'

import App from './App.vue'

const app = createApp(App)

installPlugins(app)
app.mount('#app')
