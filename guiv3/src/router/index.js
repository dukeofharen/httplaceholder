import { createRouter, createWebHashHistory } from "vue-router";
import Home from "../views/Home.vue";

const routes = [
  {
    path: "/",
    name: "Home",
    component: Home,
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
];

const router = createRouter({
  history: createWebHashHistory(),
  routes,
});

export default router;