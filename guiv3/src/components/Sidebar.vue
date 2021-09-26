<template>
  <div class="col-auto col-md-3 col-xl-2 px-md-2 px-0 bg-dark">
    <div
      class="
        d-flex
        flex-column
        align-items-center align-items-md-start
        px-3
        pt-2
        text-white
        min-vh-100
      "
    >
      <a
        href="/"
        class="
          d-flex
          align-items-center
          pb-3
          mb-md-0
          me-md-auto
          text-white text-decoration-none
        "
      >
        <span class="fs-5 d-none d-md-inline logo"
          ><img src="@/assets/logo-white_small.png" alt=""
        /></span>
      </a>
      <ul
        class="
          nav nav-pills
          flex-column
          mb-md-auto mb-0
          align-items-center align-items-sm-start
        "
        id="menu"
      >
        <SidebarMenuItem
          v-for="item of menuItems"
          :key="item.title"
          :title="item.title"
          :icon="item.icon"
          :route-name="item.routeName"
        />
      </ul>
    </div>
  </div>
</template>

<script>
import SidebarMenuItem from "@/components/SidebarMenuItem";
import getMenuItems from "@/constants/menuItems";

export default {
  name: "Sidebar",
  components: { SidebarMenuItem },
  setup() {
    return {
      menuItems: getMenuItems(),
    };
  },
};
</script>

<style lang="scss" scoped>
.logo img {
  max-width: 100%;
}
</style>
