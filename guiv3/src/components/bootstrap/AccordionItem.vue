<template>
  <div class="accordion-item">
    <h2 class="accordion-header">
      <button
        class="accordion-button"
        :class="{ collapsed: !opened }"
        type="button"
        @click="toggle"
      >
        <slot name="button-text"></slot>
      </button>
    </h2>
    <div class="accordion-collapse collapse" :class="{ show: opened }">
      <div class="accordion-body">
        <slot name="accordion-body"></slot>
      </div>
    </div>
  </div>
</template>

<script>
import { ref } from "vue";

export default {
  name: "AccordionItem",
  setup() {
    // Data
    const opened = ref(false);

    // Methods
    const toggle = () => {
      opened.value = !opened.value;
    };

    return { opened, toggle };
  },
};
</script>

<style scoped></style>
