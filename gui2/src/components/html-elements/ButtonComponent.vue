<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps({
  type: {
    type: String,
    default: 'default',
    validator(value: string): boolean {
      return ['default', 'dark', 'success', 'error', 'warning', 'purple'].includes(value)
    },
  },
  outline: {
    type: Boolean,
    default: false,
  },
})

// Data
const classMapping: any = {
  default: {
    regular: [
      'text-white',
      'bg-blue-700',
      'hover:bg-blue-800',
      'focus:ring-4',
      'focus:ring-blue-300',
      'focus:outline-none',
      'dark:focus:ring-blue-700',
    ],
    outline: [
      'text-gray-900',
      'bg-white',
      'border',
      'border-blue-700',
      'hover:text-white',
      'hover:border-blue-800',
      'hover:bg-blue-800',
      'dark:text-white',
      'dark:bg-gray-800',
      'focus:ring-4',
      'focus:ring-blue-300',
      'focus:outline-none',
      'dark:focus:ring-blue-700',
    ],
  },
}

// Computed
const classList = computed(() => {
  return classMapping[props.type][props.outline ? 'outline' : 'regular']
})
</script>

<template>
  <button
    type="button"
    class="font-medium rounded-lg text-sm px-5 py-2.5 me-2 mb-2"
    :class="classList"
  >
    <slot></slot>
  </button>
</template>

<style scoped></style>
