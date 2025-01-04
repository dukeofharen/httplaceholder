<script setup lang="ts">
import { onMounted, ref } from 'vue'
import type { ToastModel } from '@/domain/ui/toast-model.ts'
import ToastMessage from '@/components/toast/ToastMessage.vue'

// Data
const toastMessages = ref<ToastModel[]>([])

// Functions
function onToastClosed(id: number) {
  toastMessages.value = [...toastMessages.value].filter((m) => m.id !== id)
}

// Lifecycle
onMounted(() => {
  document.addEventListener('toast', function (e: CustomEventInit<ToastModel>) {
    const toast = e.detail
    if (toast) {
      toast.id = new Date().getTime()
      toastMessages.value.unshift(toast)
    }
  })
})
</script>

<template>
  <div
    v-if="toastMessages.length"
    class="fixed ms-3 right-3 top-3 flex flex-col gap-2 min-w-40 max-w-96"
  >
    <ToastMessage
      v-for="toastMessage of toastMessages"
      :key="toastMessage.id"
      :toast="toastMessage"
      @close="onToastClosed"
    />
  </div>
</template>

<style scoped></style>
