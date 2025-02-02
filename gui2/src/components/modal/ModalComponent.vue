<script setup lang="ts">
import { translate } from '@/utils/translate'
import ButtonComponent from '@/components/html-elements/ButtonComponent.vue'
import { XMarkIcon } from '@heroicons/vue/24/outline'
import { useGlobalKeyPress } from '@/composables/useGlobalKeyPress.ts'

const props = defineProps({
  title: {
    type: String,
  },
  yesText: {
    type: String,
    default: translate('general.yes'),
  },
  noText: {
    type: String,
    default: translate('general.no'),
  },
  showModal: {
    type: Boolean,
    default: false,
  },
  yesClickFunction: {
    type: Function,
  },
  noClickFunction: {
    type: Function,
  },
})
const emit = defineEmits(['closed', 'update:showModal'])
useGlobalKeyPress('Enter', () => onYesClick())
useGlobalKeyPress('Escape', () => hideModal())

// Functions
function hideModal() {
  emit('closed')
  emit('update:showModal', false)
}

function onYesClick() {
  if (!props.showModal) {
    return
  }

  if (props.yesClickFunction) {
    props.yesClickFunction()
  }

  hideModal()
}

function onNoClick() {
  if (!props.showModal) {
    return
  }

  if (props.noClickFunction) {
    props.noClickFunction()
  }

  hideModal()
}
</script>

<template>
  <div
    id="default-modal"
    tabindex="-1"
    class="overflow-y-auto overflow-x-hidden fixed top-0 right-0 left-0 z-70 justify-center items-center w-full md:inset-0 h-[calc(100%-1rem)] max-h-full cursor-pointer"
    :class="{ flex: props.showModal, hidden: !props.showModal }"
  >
    <div class="relative p-4 w-full max-w-2xl max-h-full cursor-default z-65">
      <!-- Modal content -->
      <div class="relative bg-white rounded-lg shadow-sm dark:bg-gray-700">
        <!-- Modal header -->
        <div
          class="flex items-center justify-between p-4 md:p-5 border-b rounded-t dark:border-gray-600"
        >
          <h3 class="text-xl font-semibold text-gray-900 dark:text-white">{{ props.title }}</h3>
          <button
            type="button"
            class="text-gray-400 bg-transparent hover:bg-gray-200 hover:text-gray-900 rounded-lg text-sm w-8 h-8 ms-auto inline-flex justify-center items-center dark:hover:bg-gray-600 dark:hover:text-white"
            data-modal-hide="default-modal"
            @click="hideModal"
          >
            <XMarkIcon class="size-6" />
            <span class="sr-only">{{ $translate('general.closeModal') }}</span>
          </button>
        </div>
        <!-- Modal body -->
        <div class="p-4 md:p-5 space-y-4">
          <slot></slot>
        </div>
        <!-- Modal footer -->
        <div
          class="flex items-center p-4 md:p-5 border-t border-gray-200 rounded-b dark:border-gray-600 gap-2"
        >
          <ButtonComponent type="default" @click="onYesClick">{{ props.yesText }}</ButtonComponent>
          <ButtonComponent type="dark" @click="onNoClick">{{ props.noText }}</ButtonComponent>
        </div>
      </div>
    </div>
    <div
      class="bg-gray-900/80 fixed inset-0 z-60 cursor-pointer"
      :class="{ flex: props.showModal, hidden: !props.showModal }"
      @click="hideModal"
    ></div>
  </div>
</template>

<style scoped></style>
