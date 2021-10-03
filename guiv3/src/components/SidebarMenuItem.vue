<template>
  <li class="nav-item">
    <router-link
      v-if="item.routeName"
      :to="{ name: item.routeName }"
      class="nav-link align-middle px-0 link-secondary"
      :title="item.title"
    >
      <i class="fs-5" :class="'bi-' + item.icon"></i>
      <span class="ms-4 d-none d-sm-inline menu-item-title fs-6">{{
        item.title
      }}</span>
    </router-link>
    <a
      v-if="item.onClick"
      @click="item.onClick"
      class="nav-link align-middle px-0 link-secondary"
      :title="item.title"
      ><i class="fs-5" :class="'bi-' + item.icon"></i>
      <span class="ms-4 d-none d-sm-inline menu-item-title fs-6">{{
        item.title
      }}</span></a
    >
  </li>
</template>

<script>
export default {
  name: "SidebarMenuItem",
  props: {
    item: {
      type: Object,
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
.nav-link {
  cursor: pointer;
}
</style>
