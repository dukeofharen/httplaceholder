<template>
  <div class="col-auto col-md-3 col-xl-2 px-md-2 px-0 bg-dark">
    <div class="d-flex flex-column flex-shrink-0 min-vh-100">
      <a
        href="https://httplaceholder.com"
        target="_blank"
        class="
          d-flex
          align-items-center
          pb-3
          mb-md-0
          me-md-auto
          text-white text-decoration-none
          justify-content-center
          mt-1
        "
      >
        <span class="fs-5 d-none d-md-inline logo"
          ><img src="@/assets/logo-white_small.png" alt=""
        /></span>
        <span class="fs-5 d-inline d-md-none logo"
          ><img src="@/assets/logo-white_small_square.png" alt=""
        /></span>
      </a>
      <div class="list-group list-group-flush">
        <SidebarMenuItem
          v-for="item of menuItems"
          :key="item.title"
          :item="item"
        />
      </div>
    </div>
  </div>
</template>

<script>
import SidebarMenuItem from "@/components/SidebarMenuItem";
import { computed } from "vue";
import { useStore } from "vuex";
import router from "@/router";

export default {
  name: "Sidebar",
  components: { SidebarMenuItem },
  setup() {
    const store = useStore();

    // Data
    const plainMenuItems = [
      {
        title: "Requests",
        icon: "eye",
        routeName: "Requests",
        hideWhenAuthEnabledAndNotLoggedIn: true,
      },
      {
        title: "Stubs",
        icon: "code-slash",
        routeName: "Stubs",
        hideWhenAuthEnabledAndNotLoggedIn: true,
      },
      {
        title: "Add stubs",
        icon: "plus",
        routeName: "StubForm",
        hideWhenAuthEnabledAndNotLoggedIn: true,
      },
      {
        title: "Import stubs",
        icon: "arrow-up-short",
        routeName: "ImportStubs",
        hideWhenAuthEnabledAndNotLoggedIn: true,
      },
      {
        title: "Scenarios",
        icon: "card-list",
        routeName: "Scenarios",
        hideWhenAuthEnabledAndNotLoggedIn: true,
      },
      {
        title: "Settings",
        icon: "wrench",
        routeName: "Settings",
        hideWhenAuthEnabledAndNotLoggedIn: true,
      },
      {
        title: "Log out",
        icon: "box-arrow-left",
        onlyShowWhenLoggedInAndAuthEnabled: true,
        onClick: async () => {
          store.commit("users/logOut");
          await router.push({ name: "Login" });
        },
      },
    ];

    // Computed
    const menuItems = computed(() => {
      const isAuthenticated = store.getters["users/getAuthenticated"];
      const authEnabled = store.getters["metadata/authenticationEnabled"];
      return plainMenuItems.filter(
        (i) =>
          (i.onlyShowWhenLoggedInAndAuthEnabled &&
            isAuthenticated &&
            authEnabled) ||
          (i.hideWhenAuthEnabledAndNotLoggedIn &&
            authEnabled &&
            isAuthenticated) ||
          (i.hideWhenAuthEnabledAndNotLoggedIn && !authEnabled)
      );
    });

    return {
      menuItems: menuItems,
    };
  },
};
</script>

<style lang="scss" scoped>
// Required
@import "node_modules/bootstrap/scss/functions";
@import "node_modules/bootstrap/scss/variables";
@import "node_modules/bootstrap/scss/mixins";

.logo img {
  max-width: 100%;
}
</style>
