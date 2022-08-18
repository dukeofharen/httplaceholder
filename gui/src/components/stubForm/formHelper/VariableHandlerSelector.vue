<template>
  <button class="btn btn-primary mt-2 mb-2" @click="showHandlersList = true">
    Insert variable handler
  </button>
  <div class="list-group mt-2" v-if="showHandlersList">
    <button
      v-for="item of sortedVariableParserItems"
      :key="item.name"
      class="list-group-item list-group-item-action fw-bold"
      @click="handlerSelected(item)"
    >
      <strong class="mb-1">{{ item.fullName }}</strong
      ><br />
      <small v-if="item.description">{{ item.description }}</small>
      <!-- TODO Parse YAML here -->
    </button>
  </div>
  <div class="list-group mt-2" v-if="showExamplesList">
    <button
      v-for="(example, index) of examples"
      :key="index"
      class="list-group-item list-group-item-action fw-bold"
      @click="exampleSelected(example)"
    >
      <strong class="mb-1">{{ example }}</strong>
    </button>
  </div>
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
    const examples = ref([] as string[]);
    const showHandlersList = ref(false);
    const showExamplesList = ref(false);

    // Computed
    const sortedVariableParserItems = computed(() => {
      if (!props.variableParserItems) {
        return [];
      }

      const result = props.variableParserItems;
      result.sort((a, b) => {
        if (a.fullName > b.fullName) return 1;
        if (a.fullName < b.fullName) return -1;
        return 0;
      });
      return result;
    });

    // Methods
    const handlerSelected = (handler: VariableHandlerModel) => {
      if (handler.examples.length === 1) {
        emit("exampleSelected", handler.examples[0]);
      } else {
        showExamplesList.value = true;
        examples.value = handler.examples;
      }

      showHandlersList.value = false;
    };
    const exampleSelected = (example: string) => {
      emit("exampleSelected", example);
      showExamplesList.value = false;
      showHandlersList.value = false;
    };

    return {
      sortedVariableParserItems,
      showExamplesList,
      examples,
      handlerSelected,
      showHandlersList,
      exampleSelected,
    };
  },
});
</script>

<style scoped></style>
