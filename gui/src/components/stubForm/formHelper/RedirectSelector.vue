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
import { translate } from "@/utils/translate";

export default defineComponent({
  name: "RedirectSelector",
  setup() {
    const stubFormStore = useStubFormStore();

    // Data
    const items = [
      {
        name: translate("stubForm.redirectTemporary"),
        onSelected: () => {
          stubFormStore.setDefaultTempRedirect();
          stubFormStore.closeFormHelper();
        },
      },
      {
        name: translate("stubForm.redirectPermanent"),
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
