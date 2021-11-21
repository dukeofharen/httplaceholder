<template>
  <textarea
    class="form-control"
    v-model="contents"
    @keydown.tab="simpleEditorTabPress"
    @keydown="contentsChanged"
  ></textarea>
</template>

<script>
import { ref, watch } from "vue";

export default {
  name: "SimpleEditor",
  props: {
    modelValue: {
      type: String,
      default: "",
    },
  },
  setup(props, { emit }) {
    // Data
    const contents = ref(props.modelValue);

    // Methods
    const simpleEditorTabPress = (e) => {
      if (e.key === "Tab") {
        e.preventDefault();
        const textarea = e.target;
        const [start, end] = [textarea.selectionStart, textarea.selectionEnd];
        textarea.setRangeText("  ", start, end, "end");
      }
    };
    const contentsChanged = () => {
      emit("update:modelValue", contents.value);
    };

    // Watch
    watch(props, (newProps) => (contents.value = newProps.modelValue));

    return { contents, simpleEditorTabPress, contentsChanged };
  },
};
</script>

<style scoped>
textarea {
  font-family: monospace;
  white-space: pre;
  overflow-wrap: normal;
  overflow-x: scroll;
  min-height: 300px;
}
</style>
