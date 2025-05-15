<template>
  <modal
    :title="$translate('stubForm.insertThisExample')"
    :bodyText="$translate('stubForm.youHaveUnsavedChanges')"
    :show-modal="showWarningModal"
    @close="showWarningModal = false"
    @yes-click="insert"
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
      {{ $translate('general.insert') }}
    </button>
  </div>
</template>

<script lang="ts">
import { computed, defineComponent, ref } from 'vue'
import { getExamples } from '@/utils/examples'
import { useStubFormStore } from '@/store/stubForm'
import type { ExampleModel } from '@/domain/example-model'
import { translate } from '@/utils/translate'

export default defineComponent({
  name: 'ExampleSelector',
  setup() {
    const stubFormStore = useStubFormStore()

    // Data
    const selectedExample = ref('')
    const showWarningModal = ref(false)

    // Computed
    const examples = computed(() => {
      const examplesResult = getExamples()
      examplesResult.unshift({
        stub: '',
        title: translate('stubForm.selectExample'),
        description: '',
        id: '',
      })
      return examplesResult
    })
    const example = computed<ExampleModel | undefined>(() => {
      if (!selectedExample.value) {
        return undefined
      }

      return examples.value.find((e) => e.id === selectedExample.value)
    })

    // Methods
    const preInsert = () => {
      if (!example.value) {
        return
      }

      if (stubFormStore.getFormIsDirty) {
        showWarningModal.value = true
      } else {
        insert()
      }
    }
    const insert = () => {
      if (!example.value) {
        return
      }

      stubFormStore.setInput(example.value.stub)
      stubFormStore.closeFormHelper()
    }

    return {
      insert,
      preInsert,
      examples,
      selectedExample,
      example,
      showWarningModal,
    }
  },
})
</script>

<style scoped></style>
