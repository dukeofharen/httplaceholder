<template>
  <div>
    {{ formIsDirty }}
    <select class="form-select" v-model="selectedExample">
      <option v-for="example of examples" :key="example.id" :value="example.id">
        {{ example.title }}
      </option>
    </select>
  </div>
  <div class="mt-3" v-if="exampleSelected">
    <span>{{ example.description }}</span>
    <code-highlight class="mt-2" language="yaml" :code="example.stub" />
  </div>
  <div class="mt-3">
    <button
      class="btn btn-primary"
      @click="insert"
      :disabled="!exampleSelected"
    >
      Insert
    </button>
  </div>
</template>

<script lang="ts">
import { computed, defineComponent, ref } from "vue";
import { getExamples } from "@/utils/examples";
import { useStubFormStore } from "@/store/stubForm";
import type { ExampleModel } from "@/domain/example-model";

export default defineComponent({
  name: "ExampleSelector",
  setup() {
    const stubFormStore = useStubFormStore();

    // Data
    const examples = getExamples();
    examples.unshift({
      stub: "",
      title: "Select an example...",
      description: "",
      id: "",
    });
    const selectedExample = ref("");

    // Computed
    const example = computed<ExampleModel | undefined>(() => {
      if (!selectedExample.value) {
        return {
          stub: "",
          title: "",
          id: "",
          description: "",
        };
      }

      return examples.find((e) => e.id === selectedExample.value);
    });
    const exampleSelected = computed(() => example.value && example.value.id);
    const formIsDirty = computed(() => stubFormStore.getFormIsDirty);

    // Methods
    const insert = () => {
      if (!exampleSelected.value || !example.value) {
        return;
      }

      stubFormStore.setInput(example.value.stub);
      stubFormStore.closeFormHelper();
    };

    return {
      insert,
      examples,
      selectedExample,
      example,
      exampleSelected,
      formIsDirty,
    };
  },
});
</script>

<style scoped></style>
