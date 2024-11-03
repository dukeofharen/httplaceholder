<template>
  <div class="row">
    <div class="col-md-12">
      <strong>{{ $translate("stubForm.enableDisableDynamicMode") }}</strong>
    </div>
    <div
      class="col-md-12"
      v-html="
        $vsprintf(
          $translateWithMarkdown('stubForm.enableDisableDynamicModeHint', {
            linkTarget: '_blank',
          }),
          [docUrl],
        )
      "
    />
    <div class="col-md-12 mt-3">
      <div class="form-check">
        <input
          class="form-check-input"
          type="checkbox"
          id="dynamicModeEnabled"
          v-model="dynamicModeEnabled"
        />
        <label class="form-check-label" for="dynamicModeEnabled"
          >Dynamic mode enabled</label
        >
      </div>
    </div>
    <div class="col-md-12 mt-3">
      <button class="btn btn-danger" @click="close">Close</button>
    </div>
  </div>
</template>

<script lang="ts">
import { useStubFormStore } from "@/store/stubForm";
import { computed, defineComponent } from "vue";
import { renderDocLink } from "@/utils/doc";

export default defineComponent({
  name: "SetDynamicMode",
  setup() {
    const stubFormStore = useStubFormStore();

    // Data
    const docUrl = renderDocLink("dynamic-mode");

    // Computed
    const dynamicModeEnabled = computed({
      get: () => stubFormStore.getDynamicMode,
      set: (value: boolean) => stubFormStore.setDynamicMode(value),
    });

    // Methods
    const close = () => {
      stubFormStore.closeFormHelper();
    };

    return { docUrl, dynamicModeEnabled, close };
  },
});
</script>

<style scoped></style>
