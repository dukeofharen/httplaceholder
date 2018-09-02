import Vue from 'vue'
import Router from 'vue-router'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'home',
      redirect: { name: 'requests' }
    },
    {
      path: '/requests',
      name: 'requests',
      component: () => import(/* webpackChunkName: "about" */ './views/Requests.vue')
    }
  ]
})
