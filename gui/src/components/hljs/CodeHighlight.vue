<template>
  <pre ref="codeBlock" :class="languageClass">{{ code }}</pre>
</template>

<script setup lang="ts">
import hljs from 'highlight.js'
import { computed, onMounted, ref, watch } from 'vue'

export type CodeHighlightProps = {
  language?: string
  code: string
}

const props = withDefaults(defineProps<CodeHighlightProps>(), { language: 'plaintext' })

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
