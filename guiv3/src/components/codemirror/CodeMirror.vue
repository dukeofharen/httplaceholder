<template>
  <textarea ref="editor" v-model="contents"></textarea>
</template>

<script>
import { onMounted, ref, watch } from "vue";
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
  setup(props, { emit }) {
    // Refs
    const editor = ref(null);

    // Data
    const contents = ref(props.modelValue);

    // Variables
    let cmInstance;

    // Watch
    watch(props, (newProps) => {
      if (cmInstance) {
        cmInstance.setValue(newProps.modelValue);
      }
    });

    // Lifecycle
    // TODO on destroy
    onMounted(() => {
      cmInstance = CodeMirror.fromTextArea(editor.value, props.options);
      cmInstance.on("change", () =>
        emit("update:modelValue", cmInstance.getValue())
      );
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
