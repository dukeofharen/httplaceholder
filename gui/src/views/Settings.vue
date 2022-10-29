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
      <div class="col-md-12">
        <div class="form-check">
          <input
            class="form-check-input"
            type="checkbox"
            id="storeResponses"
            v-model="storeResponses"
          />
          <label class="form-check-label" for="storeResponses"
            >Store response for request</label
          >
          <p>
            <strong>Note</strong>: this setting will be reset to its original
            value after restarting HttPlaceholder. To persist the setting, take
            a look at
            <a :href="configDocsLink" target="_blank">the documentation</a>.
          </p>
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
import { computed, defineComponent, onMounted, ref } from "vue";
import { useSettingsStore } from "@/store/settings";
import type { SettingsModel } from "@/domain/settings-model";
import { useConfigurationStore } from "@/store/configuration";
import type { ConfigurationModel } from "@/domain/stub/configuration-model";
import { handleHttpError } from "@/utils/error";
import { renderDocLink } from "@/constants/resources";

export default defineComponent({
  name: "Settings",
  setup() {
    const generalStore = useSettingsStore();
    const configurationStore = useConfigurationStore();

    // Data
    const settings = ref<SettingsModel>(generalStore.getSettings);
    const config = ref<ConfigurationModel[]>([]);

    // Functions
    const loadConfig = async () => {
      try {
        config.value = await configurationStore.getConfiguration();
      } catch (e) {
        handleHttpError(e);
      }
    };

    // Methods
    const saveSettings = () => generalStore.storeSettings(settings.value);

    // Computed
    const storeResponsesKey = "storeResponses";
    const storeResponses = computed({
      get: () => {
        const configValue = config.value.find(
          (c) => c.key === storeResponsesKey
        );
        if (!configValue) {
          return false;
        }

        return configValue.value.toLowerCase() === "true";
      },
      set: async (value) => {
        try {
          await configurationStore.updateConfigurationValue({
            configurationKey: storeResponsesKey,
            newValue: value ? "true" : "false",
          });
          await loadConfig();
        } catch (e) {
          handleHttpError(e);
        }
      },
    });
    const configDocsLink = renderDocLink("configuration");

    // Lifecycle
    onMounted(async () => await loadConfig());

    return { settings, saveSettings, config, storeResponses, configDocsLink };
  },
});
</script>

<style scoped></style>
