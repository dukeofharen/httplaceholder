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

    <div class="row mt-3">
      <div class="col-md-12">
        <h2>HttPlaceholder configuration</h2>

        <p>
          HttPlaceholder was started with the following settings. The settings
          are read-only and can only be set when starting the application.
        </p>
        <table class="table">
          <thead>
            <tr>
              <th scope="col">Setting</th>
              <th scope="col">Value</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item of config" :key="item.key">
              <td :title="item.description">{{ item.key }}</td>
              <td>{{ item.value }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, onMounted, ref } from "vue";
import { useSettingsStore } from "@/store/settings";
import type { SettingsModel } from "@/domain/settings-model";
import { useConfigurationStore } from "@/store/configuration";
import type { ConfigurationModel } from "@/domain/stub/configuration-model";

export default defineComponent({
  name: "Settings",
  setup() {
    const generalStore = useSettingsStore();
    const configurationStore = useConfigurationStore();

    // Data
    const settings = ref<SettingsModel>(generalStore.getSettings);
    const config = ref<ConfigurationModel[]>([]);

    // Methods
    const saveSettings = () => generalStore.storeSettings(settings.value);

    // Lifecycle
    onMounted(async () => {
      config.value = await configurationStore.getConfiguration();
    });

    return { settings, saveSettings, config };
  },
});
</script>

<style scoped></style>
