<template>
  <div class="row">
    <div class="col-md-12">
      <strong>Enable / disable dynamic mode</strong>
    </div>
    <div class="col-md-12">
      To learn more about the dynamic mode, read more in
      <a :href="docUrl" target="_blank">the documentation</a>.
    </div>
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
  </div>
</template>

<script lang="ts">
import { renderDocLink } from "@/constants/resources";
import { useStubFormStore } from "@/store/stubForm";
import { computed, defineComponent } from "vue";

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

    return { docUrl, dynamicModeEnabled };
  },
});
</script>

<style scoped></style>
