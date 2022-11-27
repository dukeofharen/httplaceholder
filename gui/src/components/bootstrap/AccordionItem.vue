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

<script lang="ts">
import { ref, watch } from "vue";
import { defineComponent } from "vue";

export default defineComponent({
  name: "AccordionItem",
  props: {
    opened: {
      type: null,
      default: null,
    },
  },
  setup(props, { emit }) {
    // Data
    const openedValue = ref(props.opened);

    // Methods
    const toggle = () => {
      if (props.opened !== null) {
        emit("buttonClicked");
      } else {
        if (openedValue.value === null) {
          openedValue.value = true;
          emit("opened");
        } else {
          openedValue.value = !openedValue.value;
          if (openedValue.value) {
            emit("opened");
          } else {
            emit("closed");
          }
        }
      }
    };

    watch(props, (newProps) => (openedValue.value = newProps.opened));

    return { openedValue, toggle };
  },
});
</script>

<style scoped>
.accordion-button::after {
  background-image: none;
}

.accordion-item {
  overflow-x: hidden;
}
</style>
