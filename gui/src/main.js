// General
import Vue from 'vue'
import App from './App.vue'
import router from './router'
import './registerServiceWorker'

// Fonts
import 'typeface-roboto';

// Styling
import '@/css/style.css'
import '@/css/light-theme.css'
import '@/css/dark-theme.css'

// VuejsDialog
import VuejsDialog from 'vuejs-dialog';
import 'vuejs-dialog/dist/vuejs-dialog.min.css';

// VueShortkey
import VueShortKey from 'vue-shortkey'

// FontAwesome
import 'font-awesome/css/font-awesome.css'
import 'toastr/build/toastr.css'

// CodeMirror
import VueCodemirror from 'vue-codemirror'
import '@/css/codemirror-solarized.css'
import 'codemirror/lib/codemirror.css'
import 'codemirror/mode/yaml/yaml.js'

// Filters
import './filters/yaml'
import './filters/datetime'

import './functions/stringFormat'

import store from './store/store.js'
import vuetify from './plugins/vuetify';

Vue.config.productionTip = false

Vue.use(VueCodemirror)
Vue.use(VueShortKey)
Vue.use(VuejsDialog)

new Vue({
  store,
  router,
  vuetify,
  render: h => h(App)
}).$mount('#app')
