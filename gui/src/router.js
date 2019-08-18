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
      component: () => import(/* webpackChunkName: "requests" */ './views/Requests.vue')
    },
    {
      path: '/stubs',
      name: 'stubs',
      component: () => import(/* webpackChunkName: "stubs" */ './views/Stubs.vue')
    },
    {
      path: '/login',
      name: 'login',
      component: () => import(/* webpackChunkName: "login" */ './views/Login.vue')
    },
    {
      path: '/addStub',
      name: 'addStub',
      component: () => import(/* webpackChunkName: "addStub" */ './views/AddStub.vue')
    },
    {
      path: '/updateStub/:stubId',
      name: 'updateStub',
      component: () => import(/* webpackChunkName: "updateStub" */ './views/UpdateStub.vue')
    },
    {
      path: '/downloadStubs',
      name: 'downloadStubs',
      component: () => import(/* webpackChunkName: "downloadStubs" */ './views/DownloadStubs.vue')
    }
  ]
})
