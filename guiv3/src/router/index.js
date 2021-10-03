import { createRouter, createWebHashHistory } from "vue-router";

const routes = [
  {
    path: "/",
    name: "Home",
    redirect: "/requests",
  },
  {
    path: "/requests",
    name: "Requests",
    component: () =>
      import(/* webpackChunkName: "requests" */ "../views/Requests.vue"),
  },
  {
    path: "/stubs",
    name: "Stubs",
    component: () =>
      import(/* webpackChunkName: "stubs" */ "../views/Stubs.vue"),
  },
  {
    path: "/stubForm/:stubId?",
    name: "StubForm",
    component: () =>
      import(/* webpackChunkName: "stubForm" */ "../views/StubForm.vue"),
  },
  {
    path: "/settings",
    name: "Settings",
    component: () =>
      import(/* webpackChunkName: "settings" */ "../views/Settings.vue"),
  },
];

const router = createRouter({
  history: createWebHashHistory(),
  routes,
});

export default router;
