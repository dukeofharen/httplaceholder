<template>
  <pre ref="codeBlock" class="language-yaml" :class="'language-' + language">{{
    code
  }}</pre>
</template>

<script>
import hljs from "highlight.js/lib/core";
import { onMounted, ref, watch } from "vue";

export default {
  name: "CodeHighlight",
  props: {
    language: {
      type: String,
      required: true,
    },
    code: {
      type: String,
      required: true,
    },
  },
  setup(props) {
    // Refs
    const codeBlock = ref(null);

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

    return { codeBlock };
  },
};
</script>

<style scoped></style>
