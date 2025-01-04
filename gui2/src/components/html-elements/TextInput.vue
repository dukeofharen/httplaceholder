<script setup lang="ts">
import { computed, useSlots } from 'vue'
import { XMarkIcon } from '@heroicons/vue/24/outline'

const slots = useSlots()
const props = defineProps({
  id: {
    type: String,
    required: true,
  },
  placeholder: String,
  supportClearing: Boolean,
  modelValue: {
    type: String,
  },
})
const emit = defineEmits(['update:modelValue'])

// Computed
const slotHasContent = computed(() => !!slots.default && slots.default.length > 0)

// Functions
function onInputChange(event: Event) {
  const target = event.target as HTMLInputElement
  if (target) {
    emit('update:modelValue', target.value)
  }
}

function clear() {
  if (!props.supportClearing) {
    return
  }

  emit('update:modelValue', '')
}

function onButtonClick() {
  if (props.supportClearing) {
    clear()
  }
}
</script>

<template>
  <div class="relative w-full">
    <label :for="props.id" class="sr-only">{{ props.placeholder }}</label>

    <input
      type="text"
      :id="props.id"
      :placeholder="props.placeholder"
      :value="props.modelValue"
      @input="onInputChange"
      @keyup.esc="clear"
      class="w-full rounded-md border border-gray-500 py-2.5 ps-2.5 pe-10 shadow-sm sm:text-sm dark:border-gray-500 dark:bg-gray-800 dark:text-white placeholder-gray-500 dark:placeholder-gray-300"
    />

    <span class="absolute inset-y-0 end-0 grid w-10 place-content-center">
      <button
        type="button"
        class="text-gray-600 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-300"
        @click="onButtonClick"
      >
        <span class="sr-only">{{ props.placeholder }}</span>

        <template v-if="slotHasContent">
          <slot></slot>
        </template>
        <template v-else-if="props.supportClearing">
          <XMarkIcon class="size-6" />
        </template>
      </button>
    </span>
  </div>
</template>

<style scoped></style>
