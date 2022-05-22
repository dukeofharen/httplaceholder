<template>
  <modal
    title="Insert this example?"
    bodyText="You have unsaved changes."
    :yes-click-function="insert"
    :show-modal="showWarningModal"
    @close="showWarningModal = false"
  />
  <div>
    <select class="form-select" v-model="selectedExample">
      <option v-for="example of examples" :key="example.id" :value="example.id">
        {{ example.title }}
      </option>
    </select>
  </div>
  <div class="mt-3" v-if="example">
    <span>{{ example.description }}</span>
    <code-highlight class="mt-2" language="yaml" :code="example.stub" />
  </div>
  <div class="mt-3">
    <button class="btn btn-primary" @click="preInsert" :disabled="!example">
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
    const selectedExample = ref("");
    const showWarningModal = ref(false);

    // Computed
    const examples = computed(() => {
      const examplesResult = getExamples();
      examplesResult.unshift({
        stub: "",
        title: "Select an example...",
        description: "",
        id: "",
      });
      return examplesResult;
    });
    const example = computed<ExampleModel | undefined>(() => {
      if (!selectedExample.value) {
        return undefined;
      }

      return examples.value.find((e) => e.id === selectedExample.value);
    });

    // Methods
    const preInsert = () => {
      if (!example.value) {
        return;
      }

      if (stubFormStore.getFormIsDirty) {
        showWarningModal.value = true;
      } else {
        insert();
      }
    };
    const insert = () => {
      if (!example.value) {
        return;
      }

      stubFormStore.setInput(example.value.stub);
      stubFormStore.closeFormHelper();
    };

    return {
      insert,
      preInsert,
      examples,
      selectedExample,
      example,
      showWarningModal,
    };
  },
});
</script>

<style scoped></style>
