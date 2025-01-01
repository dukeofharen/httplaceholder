import '@/style/style.css'

import { createApp } from 'vue'

import App from './App.vue'
import installPlugins from '@/plugins'

const app = createApp(App)
installPlugins(app)
app.mount('#app')
