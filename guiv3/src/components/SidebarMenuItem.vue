<template>
  <li class="nav-item">
    <router-link
      :to="{ name: routeName }"
      class="nav-link align-middle px-0 link-secondary"
      :title="title"
    >
      <i class="fs-5" :class="'bi-' + icon"></i>
      <span class="ms-4 d-none d-sm-inline menu-item-title fs-6">{{
        title
      }}</span>
    </router-link>
  </li>
</template>

<script>
export default {
  name: "SidebarMenuItem",
  props: {
    title: {
      type: String,
      required: true,
    },
    icon: {
      type: String,
      required: true,
    },
    routeName: {
      type: String,
      required: true,
    },
  },
};
</script>

<style lang="scss" scoped>
.menu-item-title {
  position: relative;
  bottom: 1px;
}
</style>
