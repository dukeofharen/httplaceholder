<template>
  <div class="list-group">
    <button
      v-for="(type, index) of types"
      :key="index"
      class="list-group-item list-group-item-action fw-bold"
      @click="lineEndingSelected(type.value)"
    >
      {{ type.name }}
    </button>
  </div>
</template>

<script lang="ts">
import { useStubFormStore } from "@/store/stubForm";
import { defineComponent } from "vue";
import { LineEndingType } from "@/domain/stub/enums/line-ending-type";
import { translate } from "@/utils/translate";

export default defineComponent({
  name: "LineEndingSelector",
  setup() {
    const stubFormStore = useStubFormStore();

    // Data
    const types = [
      {
        name: translate("stubForm.lineEndingAsProvided"),
        value: undefined,
      },
      {
        name: translate("stubForm.lineEndingUnix"),
        value: LineEndingType.Unix,
      },
      {
        name: translate("stubForm.lineEndingWindows"),
        value: LineEndingType.Windows,
      },
    ];

    // Methods
    const lineEndingSelected = (value: LineEndingType | undefined) => {
      stubFormStore.setLineEndings(value);
      stubFormStore.closeFormHelper();
    };

    return { types, lineEndingSelected };
  },
});
</script>

<style scoped></style>
