<template>
  <div class="list-group">
    <button
      v-for="(item, index) of items"
      :key="index"
      class="list-group-item list-group-item-action fw-bold"
      @click="item.onSelected"
    >
      {{ item.name }}
    </button>
  </div>
</template>

<script lang="ts">
import { useStubFormStore } from "@/store/stubForm";
import { defineComponent } from "vue";

export default defineComponent({
  name: "RedirectSelector",
  setup() {
    const stubFormStore = useStubFormStore();

    // Data
    const items = [
      {
        name: "Temporary redirect",
        onSelected: () => {
          stubFormStore.setDefaultTempRedirect();
          stubFormStore.closeFormHelper();
        },
      },
      {
        name: "Permanent redirect",
        onSelected: () => {
          stubFormStore.setDefaultPermanentRedirect();
          stubFormStore.closeFormHelper();
        },
      },
    ];

    return { items };
  },
});
</script>
