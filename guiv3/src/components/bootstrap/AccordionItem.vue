<template>
  <div class="accordion-item">
    <h2 class="accordion-header">
      <button
        class="accordion-button"
        :class="{ collapsed: !openedValue }"
        type="button"
        @click="toggle"
      >
        <slot name="button-text"></slot>
      </button>
    </h2>
    <slide-up-down v-model="openedValue" :duration="300">
      <div class="accordion-collapse collapse show">
        <div class="accordion-body">
          <slot name="accordion-body"></slot>
        </div>
      </div>
    </slide-up-down>
  </div>
</template>

<script>
import { ref, watch } from "vue";

export default {
  name: "AccordionItem",
  props: {
    opened: {
      type: Boolean,
      default: false,
    },
  },
  setup(props, { emit }) {
    // Data
    const openedValue = ref(props.opened);

    // Methods
    const toggle = () => {
      emit("buttonClicked");
      // TODO
      // opened.value = !opened.value;
      // if (opened.value) {
      //   emit("opened");
      // } else {
      //   emit("closed");
      // }
    };

    watch(props, (newProps) => (openedValue.value = newProps.opened));

    return { openedValue, toggle };
  },
};
</script>

<style scoped></style>
