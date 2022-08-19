<template>
  <button class="btn btn-primary mt-2 mb-2" @click="showHandlersList = true">
    Insert variable handler
  </button>
  <div class="list-group mt-2" v-if="showHandlersList">
    <span class="list-group-item list-group-item-action fw-bold"
      >Select a variable handler to insert in the response...</span
    >
    <button
      v-for="item of sortedVariableParserItems"
      :key="item.name"
      class="list-group-item list-group-item-action fw-bold"
      @click="handlerSelected(item)"
    >
      <strong class="mb-1">{{ item.fullName }}</strong
      ><br />
      <small v-if="item.description" v-html="parseMarkdown(item.description)" />
    </button>
  </div>
  <div class="list-group mt-2" v-if="showExamplesList">
    <small
      v-if="selectedHandler"
      v-html="parseMarkdown(selectedHandler.description)"
    />
    <span class="list-group-item list-group-item-action fw-bold"
      >Select an example to insert in the response...</span
    >
    <div class="list-group-item list-group-item-action fw-bold">
      <input
        type="text"
        class="form-control"
        placeholder="Filter examples..."
        v-model="exampleFilter"
        ref="exampleFilterInput"
      />
    </div>
    <button
      v-for="(example, index) of filteredExamples"
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
import { marked } from "marked";
import { useStubFormStore } from "@/store/stubForm";

export default defineComponent({
  name: "VariableHandlerSelector",
  props: {
    variableParserItems: {
      type: Array as PropType<VariableHandlerModel[]>,
    },
  },
  emits: {
    exampleSelected() {
      return true;
    },
  },
  setup(props, { emit }) {
    const stubFormStore = useStubFormStore();

    // Refs
    const exampleFilterInput = ref<HTMLInputElement>();

    // Data
    const examples = ref([] as string[]);
    const showHandlersList = ref(false);
    const showExamplesList = ref(false);
    const selectedHandler = ref<VariableHandlerModel | null>();
    const exampleFilter = ref("");

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
    const filteredExamples = computed(() => {
      if (!exampleFilter.value) {
        return examples.value;
      }

      return examples.value.filter((e) =>
        e.toLowerCase().includes(exampleFilter.value.toLowerCase())
      );
    });

    // Methods
    const handlerSelected = (handler: VariableHandlerModel) => {
      if (handler.examples.length === 1) {
        emit("exampleSelected", handler.examples[0]);
        stubFormStore.setDynamicMode(true);
      } else {
        showExamplesList.value = true;
        selectedHandler.value = handler;
        examples.value = handler.examples;
        setTimeout(() => {
          if (exampleFilterInput.value) {
            exampleFilterInput.value?.focus();
          }
        }, 10);
      }

      showHandlersList.value = false;
    };
    const exampleSelected = (example: string) => {
      emit("exampleSelected", example);
      showExamplesList.value = false;
      showHandlersList.value = false;
      stubFormStore.setDynamicMode(true);
      selectedHandler.value = null;
      exampleFilter.value = "";
    };
    const parseMarkdown = (input: string) => {
      return marked.parse(input);
    };

    return {
      sortedVariableParserItems,
      showExamplesList,
      examples,
      handlerSelected,
      showHandlersList,
      exampleSelected,
      parseMarkdown,
      selectedHandler,
      exampleFilter,
      exampleFilterInput,
      filteredExamples,
    };
  },
});
</script>

<style scoped></style>
