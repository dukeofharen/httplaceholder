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

    // Methods
    const initializeCodemirror = () => {
      cmInstance = CodeMirror.fromTextArea(editor.value, props.options);
      cmInstance.on("change", () =>
        emit("update:modelValue", cmInstance.getValue())
      );
      cmInstance.setOption("extraKeys", {
        Tab: (cm) => {
          // Make sure inserts spaces instead of tabs.
          const spaces = Array(cm.getOption("indentUnit") + 1).join(" ");
          cm.replaceSelection(spaces);
        },
      });
    };

    // Watch
    watch(
      () => props.modelValue,
      (newModelValue) => {
        if (cmInstance && cmInstance.getValue() !== newModelValue) {
          cmInstance.setValue(newModelValue);
        }
      }
    );
    watch(
      () => props.options,
      () => {
        if (cmInstance && props.options) {
          const cleanOptions = JSON.parse(JSON.stringify(props.options));
          for (const key of Object.keys(cleanOptions)) {
            cmInstance.setOption(key, cleanOptions[key]);
          }
        }
      },
      { deep: true }
    );

    // Lifecycle
    onMounted(() => initializeCodemirror());

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
