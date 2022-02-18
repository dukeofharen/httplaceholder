<template>
  <pre ref="codeBlock" :class="languageClass">{{ code }}</pre>
</template>

<script>
import hljs from "highlight.js/lib/core";
import { computed, onMounted, ref, watch } from "vue";
import { defineComponent } from "vue";

export default defineComponent({
  name: "CodeHighlight",
  props: {
    language: {
      type: String,
      required: true,
      default: "plaintext",
    },
    code: {
      type: String,
      required: true,
    },
  },
  setup(props) {
    // Refs
    const codeBlock = ref(null);

    // Computed
    const languageClass = computed(() => props.language);

    // Functions
    const reloadCode = () => {
      setTimeout(() => {
        if (codeBlock.value) {
          hljs.highlightElement(codeBlock.value);
        }
      }, 10);
    };

    // Lifecycle
    onMounted(() => reloadCode());

    // Watch
    watch(
      () => props.code,
      () => reloadCode()
    );

    return { codeBlock, languageClass };
  },
});
</script>

<style scoped></style>
