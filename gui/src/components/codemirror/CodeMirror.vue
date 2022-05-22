<template>
  <textarea ref="editor" v-model="contents"></textarea>
</template>

<script lang="ts">
import { onMounted, ref, watch } from "vue";
import CodeMirror from "codemirror";
import { useSettingsStore } from "@/store/settings";
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
    const generalStore = useSettingsStore();

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
        cmInstance.on("change", () => {
          if (props.modelValue !== cmInstance.getValue()) {
            emit("update:modelValue", cmInstance.getValue());
          }
        });
        cmInstance.setOption("extraKeys", {
          Tab: (cm) => {
            if (cm.somethingSelected()) {
              cm.indentSelection("add");
            } else {
              // Make sure inserts spaces instead of tabs.
              const currentIndent = cm.getOption("indentUnit") || 0;
              const spaces = Array(currentIndent + 1).join(" ");
              cm.replaceSelection(spaces);
            }
          },
          "Shift-Tab": (cm) => {
            if (cm.somethingSelected()) {
              cm.indentSelection("subtract");
            }
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
            (cmInstance as any).setOption(key, cleanOptions[key]);
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
