import Vue from 'vue'
import Router from 'vue-router'
import Home from './views/Home.vue'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home
    },
    {
      path: '/requests',
      name: 'requests',
      component: () => import(/* webpackChunkName: "about" */ './views/Requests.vue')
    }
  ]
})
