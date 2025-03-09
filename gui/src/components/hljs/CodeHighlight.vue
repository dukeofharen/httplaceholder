<template>
  <pre ref="codeBlock" :class="languageClass">{{ code }}</pre>
</template>

<script setup lang="ts">
import hljs from 'highlight.js'
import { computed, onMounted, ref, watch } from 'vue'

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

// Data
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

<style scoped></style>
