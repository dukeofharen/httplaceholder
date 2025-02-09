<template>
  <router-link
    v-if="item.routeName"
    :to="{ name: item.routeName }"
    class="list-group-item list-group-item-action menu-item"
    :title="item.title"
  >
    <i class="fs-5" :class="'bi-' + item.icon"></i>
    <strong class="mb-1 d-none d-sm-inline ms-3">{{ item.title }}</strong>
  </router-link>
  <a
    v-if="item.onClick"
    class="list-group-item list-group-item-action menu-item"
    :title="item.title"
    @click="item.onClick"
  >
    <i class="fs-5" :class="'bi-' + item.icon"></i>
    <strong class="mb-1 d-none d-md-inline ms-3">{{ item.title }}</strong>
  </a>
  <a
    v-if="item.url"
    class="list-group-item list-group-item-action menu-item"
    :title="item.title"
    :href="item.url"
    :target="item.targetBlank ? '_blank' : ''"
  >
    <i class="fs-5" :class="'bi-' + item.icon"></i>
    <strong class="mb-1 d-none d-md-inline ms-3">{{ item.title }}</strong>
  </a>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "SidebarMenuItem",
  props: {
    item: {
      type: Object,
      required: true,
    },
  },
});
</script>

<style lang="scss" scoped>
@import "@/style/bootstrap";

.menu-item {
  cursor: pointer;
}

.list-group .list-group-item {
  background-color: $gray-900;
  color: $gray-400;
  border: 0 !important;
}

.router-link-active {
  background-color: $green-700 !important;
}

@include media-breakpoint-down(sm) {
  .list-group-item {
    text-align: center;
  }
}
</style>
