<template>
  <div>
    <h1>Settings</h1>

    <div class="row">
      <div class="col-md-12">
        <h2>Features</h2>
        <div class="form-check">
          <input
            class="form-check-input"
            type="checkbox"
            id="darkTheme"
            v-model="settings.darkTheme"
            @change="saveSettings"
          />
          <label class="form-check-label" for="darkTheme">Dark theme</label>
        </div>
      </div>
      <div class="col-md-12">
        <div class="form-check">
          <input
            class="form-check-input"
            type="checkbox"
            id="saveSearchFilters"
            v-model="settings.saveSearchFilters"
            @change="saveSettings"
          />
          <label class="form-check-label" for="saveSearchFilters"
            >Persist search filters on stubs and request screens</label
          >
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { ref } from "vue";
import { useSettingsStore } from "@/store/settings";
import { defineComponent } from "vue";
import type { SettingsModel } from "@/domain/settings-model";

export default defineComponent({
  name: "Settings",
  setup() {
    const generalStore = useSettingsStore();

    // Data
    const settings = ref<SettingsModel>(generalStore.getSettings);

    // Methods
    const saveSettings = () => generalStore.storeSettings(settings.value);

    return { settings, saveSettings };
  },
});
</script>

<style scoped></style>
