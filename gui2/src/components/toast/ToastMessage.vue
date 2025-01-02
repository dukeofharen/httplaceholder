<script setup lang="ts">
import { XMarkIcon } from '@heroicons/vue/24/outline'
import { computed, onMounted, type PropType } from 'vue'
import type { ToastModel } from '@/domain/toast-model.ts'
import { defaultToastDuration } from '@/constants.ts'

const props = defineProps({
  toast: {
    type: Object as PropType<ToastModel>,
  },
})
const emit = defineEmits(['close'])

// Computed
const classList = computed(() => {
  const result = []
  switch (props.toast?.type) {
    case 'success':
      result.push('bg-green-700')
      break
    case 'warning':
      result.push('bg-yellow-700')
      break
    case 'error':
      result.push('bg-red-700')
      break
  }

  return result
})

// Functions
function close() {
  emit('close', props.toast?.id)
}

// Lifecycle
onMounted(() => {
  setTimeout(() => close(), props.toast?.duration ?? defaultToastDuration)
})
</script>

<template>
  <div
    role="alert"
    class="rounded-xl p-4 cursor-pointer text-white"
    :class="classList"
    @click="close"
  >
    <div class="flex items-start gap-4">
      <div class="flex-1">
        <strong class="block font-medium">{{ props.toast?.title }}</strong>

        <p v-if="props.toast?.message" class="mt-1 text-sm">{{ props.toast?.message }}</p>
      </div>

      <button class="transition">
        <span class="sr-only">{{ $translate('general.dismissPopup') }}</span>

        <XMarkIcon class="size-6" />
      </button>
    </div>
  </div>
</template>

<style scoped></style>
