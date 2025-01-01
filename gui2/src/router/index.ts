import { createRouter, createWebHashHistory, type RouteRecordRaw } from 'vue-router'
import { useUsersStore } from '@/stores/users'
import { useMetadataStore } from '@/stores/metadata'

const routes = [
  {
    path: '/',
    name: 'Home',
    redirect: '/requests',
  },
  {
    path: '/requests',
    name: 'Requests',
    component: () => import(/* webpackChunkName: "requests" */ '../views/RequestsPage.vue'),
  },
  {
    path: '/stubs',
    name: 'Stubs',
    component: () => import(/* webpackChunkName: "stubs" */ '../views/StubsPage.vue'),
  },
  // {
  //   path: "/stubForm/:stubId?",
  //   name: "StubForm",
  //   component: () =>
  //     import(/* webpackChunkName: "stubForm" */ "../views/StubForm.vue"),
  // },
  // {
  //   path: "/importStubs",
  //   name: "ImportStubs",
  //   component: () =>
  //     import(/* webpackChunkName: "importStubs" */ "../views/ImportStubs.vue"),
  // },
  // {
  //   path: "/settings",
  //   name: "Settings",
  //   component: () =>
  //     import(/* webpackChunkName: "settings" */ "../views/Settings.vue"),
  // },
  // {
  //   path: "/login",
  //   name: "Login",
  //   component: () =>
  //     import(/* webpackChunkName: "login" */ "../views/Login.vue"),
  // },
  // {
  //   path: "/scenarios",
  //   name: "Scenarios",
  //   component: () =>
  //     import(/* webpackChunkName: "scenarios" */ "../views/Scenarios.vue"),
  // },
  // {
  //   path: "/scenarioForm/:scenario?",
  //   name: "ScenarioForm",
  //   component: () =>
  //     import(
  //       /* webpackChunkName: "scenarioForm" */ "../views/ScenarioForm.vue"
  //       ),
  // },
] as RouteRecordRaw[]

const router = createRouter({
  history: createWebHashHistory(),
  routes,
})

router.beforeEach((to, _, next) => {
  const usersStore = useUsersStore()
  const metadataStore = useMetadataStore()
  const authEnabled = metadataStore.getAuthenticationEnabled
  const authenticated = usersStore.getAuthenticated
  if (authEnabled && !authenticated && to.name !== 'Login') {
    next({ name: 'Login' })
  } else {
    next()
  }
})

export default router
