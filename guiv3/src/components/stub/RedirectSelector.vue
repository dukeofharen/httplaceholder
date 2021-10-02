<template>
  <div class="list-group">
    <button
      v-for="item of items"
      :key="item.key"
      class="list-group-item list-group-item-action fw-bold"
      @click="item.onSelected"
    >
      {{ item.name }}
    </button>
  </div>
</template>

<script>
import { useStore } from "vuex";

export default {
  name: "RedirectSelector",
  setup() {
    const store = useStore();

    // Data
    const items = [
      {
        key: "temp",
        name: "Temporary redirect",
        onSelected: () => {
          store.commit("stubForm/setDefaultTempRedirect");
          store.commit("stubForm/closeFormHelper");
        },
      },
      {
        key: "permanent",
        name: "Permanent redirect",
        onSelected: () => {
          store.commit("stubForm/setDefaultPermanentRedirect");
          store.commit("stubForm/closeFormHelper");
        },
      },
    ];

    return { items };
  },
};
</script>
