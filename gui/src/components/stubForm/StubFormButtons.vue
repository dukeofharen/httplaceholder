<template>
  <button class="btn btn-success me-2 btn-mobile" @click="save">
    {{ $translate('general.save') }}
  </button>
  <button v-if="showSaveAsNewStubButton" class="btn btn-success me-2 btn-mobile" @click="addStub">
    {{ $translate('stubForm.saveAsNewStub') }}
  </button>
  <button type="button" class="btn btn-danger btn-mobile" @click="showResetModal = true">
    {{ $translate('general.reset') }}
  </button>
  <modal
    :title="$translate('stubForm.resetToDefaults')"
    :show-modal="showResetModal"
    @close="showResetModal = false"
    @yes-click="reset"
  />
</template>

<script lang="ts">
import { computed, defineComponent, ref } from 'vue'
import { handleHttpError } from '@/utils/error'
import { useRoute, useRouter } from 'vue-router'
import { error, success } from '@/utils/toast'
import { useStubsStore } from '@/store/stubs'
import { useStubFormStore } from '@/store/stubForm'
import { FormHelperKey } from '@/domain/stubForm/form-helper-key'
import { vsprintf } from 'sprintf-js'
import { defaultStub } from '@/strings/exmaples'
import { translate } from '@/utils/translate'
import { useSaveMagicKeys } from '@/composables/useSaveMagicKeys.ts'

export default defineComponent({
  name: 'StubFormButtons',
  props: {
    modelValue: {
      type: String,
      default: '',
    },
  },
  setup(props, { emit }) {
    const stubStore = useStubsStore()
    const stubFormStore = useStubFormStore()
    const route = useRoute()
    const router = useRouter()

    // Data
    const showResetModal = ref(false)

    // Computed
    const input = computed({
      get: () => stubFormStore.getInput,
      set: (value) => stubFormStore.setInput(value),
    })
    const newStub = computed(() => !route.params.stubId)
    const stubId = computed(() => route.params.stubId as string)
    const showSaveAsNewStubButton = computed(
      () => !stubFormStore.getInputHasMultipleStubs && !newStub.value,
    )

    // Methods
    const reset = async () => {
      input.value = defaultStub
      emit('update:modelValue', '')
      await router.push({ name: 'StubForm' })
    }
    const addStub = async () => {
      try {
        const result = await stubStore.addStubs(input.value)
        if (result.length === 1) {
          const addedStubId = result[0].stub.id
          if (stubId.value !== addedStubId) {
            await router.push({
              name: 'StubForm',
              params: { stubId: addedStubId },
            })
          }
        }

        success(translate('stubs.stubsAddedSuccessfully'))
      } catch (e) {
        if (!handleHttpError(e)) {
          error(vsprintf(translate('errors.errorDuringParsingOfYaml'), [e]))
        }
      }
    }
    const updateStub = async () => {
      try {
        await stubStore.updateStub({
          stubId: stubId.value,
          input: input.value,
        })
        success(translate('stubs.stubUpdatedSuccessfully'))
        const currentStubId = stubFormStore.getStubId
        if (stubId.value !== currentStubId) {
          await router.push({
            name: 'StubForm',
            params: { stubId: currentStubId },
          })
        }
      } catch (e) {
        if (!handleHttpError(e)) {
          error(vsprintf(translate('errors.errorDuringParsingOfYaml'), [e]))
        }
      }
    }
    const save = async () => {
      if (newStub.value || !showSaveAsNewStubButton.value) {
        await addStub()
      } else {
        await updateStub()
      }
    }

    // Lifecycle
    const { registerSaveFunction } = useSaveMagicKeys()
    registerSaveFunction(async () => {
      const currentSelectedFormHelper = stubFormStore.getCurrentSelectedFormHelper
      if (currentSelectedFormHelper !== FormHelperKey.ResponseBody) {
        await save()
      }
    })

    return {
      showResetModal,
      reset,
      stubId,
      save,
      newStub,
      addStub,
      showSaveAsNewStubButton,
    }
  },
})
</script>

<style scoped></style>
