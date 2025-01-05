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
  dense: {
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
      'text-blue-900',
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
  dark: {
    regular: [
      'text-white',
      'bg-black',
      'hover:bg-gray-800',
      'focus:ring-4',
      'focus:ring-gray-300',
      'focus:outline-none',
      'dark:focus:ring-gray-700',
    ],
    outline: [
      'text-gray-900',
      'border',
      'border-black',
      'hover:text-white',
      'hover:border-black',
      'hover:bg-black',
      'dark:text-white',
      'focus:ring-4',
      'focus:ring-gray-300',
      'focus:outline-none',
      'dark:focus:ring-gray-700',
    ],
  },
  success: {
    regular: [
      'text-white',
      'bg-green-900',
      'hover:bg-green-800',
      'focus:ring-4',
      'focus:ring-green-300',
      'focus:outline-none',
      'dark:focus:ring-green-700',
    ],
    outline: [
      'text-green-900',
      'bg-white',
      'border',
      'border-green-700',
      'hover:text-white',
      'hover:border-green-800',
      'hover:bg-green-800',
      'dark:text-white',
      'dark:bg-gray-800',
      'focus:ring-4',
      'focus:ring-green-300',
      'focus:outline-none',
      'dark:focus:ring-green-700',
    ],
  },
  error: {
    regular: [
      'text-white',
      'bg-red-700',
      'hover:bg-red-800',
      'focus:ring-4',
      'focus:ring-red-300',
      'focus:outline-none',
      'dark:focus:ring-red-700',
    ],
    outline: [
      'text-red-900',
      'bg-white',
      'border',
      'border-red-700',
      'hover:text-white',
      'hover:border-red-800',
      'hover:bg-red-800',
      'dark:text-white',
      'dark:bg-gray-800',
      'focus:ring-4',
      'focus:ring-red-300',
      'focus:outline-none',
      'dark:focus:ring-red-700',
    ],
  },
  warning: {
    regular: [
      'text-white',
      'bg-yellow-700',
      'hover:bg-yellow-800',
      'focus:ring-4',
      'focus:ring-yellow-300',
      'focus:outline-none',
      'dark:focus:ring-yellow-700',
    ],
    outline: [
      'text-yellow-900',
      'bg-white',
      'border',
      'border-yellow-700',
      'hover:text-white',
      'hover:border-yellow-800',
      'hover:bg-yellow-800',
      'dark:text-white',
      'dark:bg-gray-800',
      'focus:ring-4',
      'focus:ring-yellow-300',
      'focus:outline-none',
      'dark:focus:ring-yellow-700',
    ],
  },
  purple: {
    regular: [
      'text-white',
      'bg-purple-700',
      'hover:bg-purple-800',
      'focus:ring-4',
      'focus:ring-purple-300',
      'focus:outline-none',
      'dark:focus:ring-purple-700',
    ],
    outline: [
      'text-purple-900',
      'bg-white',
      'border',
      'border-purple-700',
      'hover:text-white',
      'hover:border-purple-800',
      'hover:bg-purple-800',
      'dark:text-white',
      'dark:bg-gray-800',
      'focus:ring-4',
      'focus:ring-purple-300',
      'focus:outline-none',
      'dark:focus:ring-purple-700',
    ],
  },
}

// Computed
const classList = computed(() => {
  let result = [...classMapping[props.type][props.outline ? 'outline' : 'regular']]
  if (props.dense) {
    result = ['px-2', 'py-1', 'text-xs'].concat(result)
  } else {
    result = ['px-5', 'py-2.5'].concat(result)
  }

  return result
})
</script>

<template>
  <button
    type="button"
    class="rounded-lg text-sm flex justify-center gap-1.5 items-center"
    :class="classList"
  >
    <slot></slot>
  </button>
</template>

<style scoped></style>
