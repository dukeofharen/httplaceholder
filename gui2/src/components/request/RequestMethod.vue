<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps({
  method: {
    type: String,
    required: true,
  },
})

// Computed
const className = computed(() => {
  const result = []
  switch (props.method.toLowerCase()) {
    case 'get':
      result.push('text-green-600')
      break
    case 'post':
      result.push('text-blue-600')
      break
    case 'put':
      result.push('text-orange-500')
      break
    case 'delete':
      result.push('text-yellow-400')
      break
    case 'options':
      result.push('text-purple-700')
      break
    case 'patch':
      result.push('text-pink-700')
      break
    case 'head':
      result.push('text-red-900')
      break
  }

  return result
})
</script>

<template>
  <span class="font-bold" :class="className">{{ props.method }}</span>
</template>

<style scoped></style>
