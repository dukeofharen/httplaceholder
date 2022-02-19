<template>
  <textarea ref="editor" v-model="contents"></textarea>
</template>

<script lang="ts">
import { onMounted, ref, watch } from "vue";
import CodeMirror from "codemirror";
import { useGeneralStore } from "@/store/general";
import { defineComponent } from "vue";

export default defineComponent({
  name: "CodeMirror",
  props: {
    modelValue: {
      type: String,
      default: "",
    },
    options: {
      type: Object,
      default: () => ({}),
    },
  },
  setup(props, { emit }) {
    const generalStore = useGeneralStore();

    // Refs
    const editor = ref<HTMLTextAreaElement>();

    // Data
    const contents = ref(props.modelValue);

    // Variables
    let cmInstance: CodeMirror.EditorFromTextArea;

    // Methods
    const initializeCodemirror = () => {
      if (editor.value) {
        cmInstance = CodeMirror.fromTextArea(editor.value, props.options);
        cmInstance.on("change", () =>
          emit("update:modelValue", cmInstance.getValue())
        );
        cmInstance.setOption("extraKeys", {
          Tab: (cm) => {
            // Make sure inserts spaces instead of tabs.
            const currentIndent = cm.getOption("indentUnit") || 0;
            const spaces = Array(currentIndent + 1).join(" ");
            cm.replaceSelection(spaces);
          },
        });
        if (generalStore.getDarkTheme) {
          cmInstance.setOption("theme", "material-darker");
        }
      }
    };
    const replaceSelection = (replacement: string, selection: string) => {
      if (cmInstance) {
        cmInstance.replaceSelection(replacement, selection);
      }
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
            (cmInstance as any).setOption(key, cleanOptions[key]); // TODO make this cleaner.
          }
        }
      },
      { deep: true }
    );

    // Lifecycle
    onMounted(() => initializeCodemirror());

    return { contents, editor, replaceSelection };
  },
});
</script>

<style lang="scss" scoped>
.editor-wrapper {
  width: 100%;
  overflow-x: hidden;
}
</style>
