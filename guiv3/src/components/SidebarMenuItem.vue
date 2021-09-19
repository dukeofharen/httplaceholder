<template>
  <li class="nav-item">
    <router-link
      :to="{ name: routeName }"
      class="nav-link align-middle px-0"
      :title="title"
    >
      <i class="fs-4" :class="'bi-' + icon"></i>
      <span class="ms-3 d-none d-sm-inline menu-item-title">{{ title }}</span>
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
  bottom: 3px;
}
</style>
