import Vue from "vue";
import Router from "vue-router";
import { routeNames } from "@/router/routerConstants";

Vue.use(Router);

export default new Router({
  routes: [
    {
      path: "/",
      name: routeNames.home,
      redirect: { name: "requests" }
    },
    {
      path: "/requests",
      name: routeNames.requests,
      component: () =>
        import(/* webpackChunkName: "requests" */ "@/views/Requests.vue")
    },
    {
      path: "/stubs",
      name: routeNames.stubs,
      component: () =>
        import(/* webpackChunkName: "stubs" */ "@/views/Stubs.vue")
    },
    {
      path: "/login",
      name: routeNames.login,
      component: () =>
        import(/* webpackChunkName: "login" */ "@/views/Login.vue")
    },
    {
      path: "/addStub",
      name: routeNames.addStub,
      component: () =>
        import(/* webpackChunkName: "addStub" */ "@/views/AddStub.vue")
    },
    {
      path: "/updateStub/:stubId",
      name: routeNames.updateStub,
      component: () =>
        import(/* webpackChunkName: "updateStub" */ "@/views/UpdateStub.vue")
    },
    {
      path: "/downloadStubs",
      name: routeNames.downloadStubs,
      component: () =>
        import(
          /* webpackChunkName: "downloadStubs" */ "@/views/DownloadStubs.vue"
        )
    },
    {
      path: "/settings",
      name: routeNames.settings,
      component: () =>
        import(/* webpackChunkName: "settings" */ "@/views/Settings.vue")
    }
  ]
});
