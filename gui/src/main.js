import Vue from 'vue'
import App from './App.vue'
import router from './router'
import './registerServiceWorker'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap/dist/css/bootstrap-grid.css'
import 'bootstrap/dist/css/bootstrap-reboot.css'
import 'font-awesome/css/font-awesome.css'
import 'toastr/build/toastr.css'

import './filters/yaml'
import './filters/datetime'

import './functions/stringFormat'

Vue.config.productionTip = false

new Vue({
  router,
  render: h => h(App)
}).$mount('#app')
