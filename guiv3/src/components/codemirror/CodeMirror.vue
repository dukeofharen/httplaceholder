<template>
  <textarea ref="editor"></textarea>
</template>

<script>
import { onMounted, ref } from "vue";
import CodeMirror from "codemirror";

export default {
  name: "CodeMirror",
  props: {
    modelValue: {
      type: String,
      default: "",
    },
    options: {
      type: Object,
      default: () => {},
    },
  },
  setup(props) {
    // Refs
    const editor = ref(null);

    // Data
    const contents = ref(props.modelValue);

    // Watch
    // watch(contents, (newContents) => {
    //   emit("update:modelValue", newContents);
    // });

    // Lifecycle
    // TODO on destroy
    onMounted(() => {
      CodeMirror.fromTextArea(editor.value, props.options);
    });

    return { contents, editor };
  },
};
</script>

<style lang="scss" scoped>
.editor-wrapper {
  width: 100%;
  overflow-x: hidden;
}
</style>
