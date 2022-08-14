<template>
  <select
    class="form-select"
    @change="onHandlerSelected($event)"
    v-model="selectedVariableHandler"
  >
    <option value="">
      Select a variable handler to insert in the response...
    </option>
    <option
      v-for="item of sortedVariableParserItems"
      :key="item.key"
      :value="item.key"
    >
      {{ item.name }}
    </option>
  </select>
</template>

<script lang="ts">
import { computed, defineComponent, type PropType, ref } from "vue";
import type { VariableHandlerModel } from "@/domain/metadata/variable-handler-model";

export default defineComponent({
  name: "VariableHandlerSelector",
  props: {
    variableParserItems: {
      type: Array as PropType<VariableHandlerModel[]>,
    },
  },
  setup(props, { emit }) {
    // Data
    const selectedVariableHandler = ref("");

    // Computed
    const sortedVariableParserItems = computed(() => {
      if (!props.variableParserItems) {
        return [];
      }

      const result = props.variableParserItems.map((h) => ({
        key: h.name,
        name: h.fullName,
        example: h.example,
      }));

      result.sort((a, b) => {
        if (a.name > b.name) return 1;
        if (a.name < b.name) return -1;
        return 0;
      });

      return result;
    });

    // Methods
    const onHandlerSelected = (event: Event) => {
      const element: HTMLInputElement = event.target as HTMLInputElement;
      const handler = props.variableParserItems?.find(
        (h) => h.name === element.value
      );
      if (handler) {
        if (handler.examples.length === 1) {
          emit("handlerSelected", handler.examples[0]);
          setTimeout(() => (selectedVariableHandler.value = ""), 10);
        }
      }
    };

    return {
      sortedVariableParserItems,
      onHandlerSelected,
      selectedVariableHandler,
    };
  },
});
</script>

<style scoped></style>
