<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import hljs from 'highlight.js/lib/core'

const props = defineProps({
  language: {
    type: String,
    default: 'plaintext',
  },
  code: {
    type: String,
    required: true,
  },
})

// Refs
const codeBlock = ref<HTMLElement>()

// Computed
const languageClass = computed(() => props.language)

// Functions
const reloadCode = () => {
  setTimeout(() => {
    if (codeBlock.value) {
      hljs.highlightElement(codeBlock.value)
    }
  }, 10)
}

// Lifecycle
onMounted(() => reloadCode())

// Watch
watch(
  () => props.code,
  () => reloadCode(),
)
</script>

<template>
  <pre ref="codeBlock" :class="languageClass">{{ code }}</pre>
</template>
