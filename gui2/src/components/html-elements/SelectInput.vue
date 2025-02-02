<script setup lang="ts">
import { computed, type PropType, useSlots } from 'vue'
import { XMarkIcon } from '@heroicons/vue/24/outline'
import type { SelectListItem } from '@/domain/ui/select-list-item.ts'

const slots = useSlots()
const props = defineProps({
  id: {
    type: String,
    required: true,
  },
  placeholder: String,
  supportClearing: Boolean,
  items: Array as PropType<SelectListItem[]>,
  modelValue: {
    type: [String, Number],
  },
})
const emit = defineEmits(['update:modelValue'])

// Computed
const slotHasContent = computed(() => !!slots.default && slots.default.length > 0)
const classList = computed(() => {
  const result = []
  if (!props.modelValue) {
    result.push('text-gray-500')
    result.push('dark:text-gray-300')
  }

  return result
})

// Functions
function onInputChange(event: Event) {
  const target = event.target as HTMLInputElement
  if (target) {
    const val = !isNaN(props.modelValue as any) ? parseInt(target.value) : target.value
    emit('update:modelValue', val)
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

    <select
      :id="props.id"
      :value="props.modelValue"
      @input="onInputChange"
      @keyup.esc="clear"
      class="w-full rounded-md border border-gray-500 py-2.5 ps-2.5 pe-10 shadow-xs bg-white sm:text-sm dark:border-gray-500 dark:bg-gray-800 appearance-none"
      :class="classList"
    >
      <option v-if="props.placeholder" value="">{{ props.placeholder }}</option>
      <option v-for="item of items" :key="item.value" :value="item.value">{{ item.label }}</option>
    </select>

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
