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
    <div class="row">
      <div class="col-md-12">
        <label for="requestPageSize"
          >Default number of requests on the request page (set to 0 to disable
          request paging)</label
        >
        <input
          type="number"
          class="form-control mt-2"
          id="requestPageSize"
          v-model="settings.requestPageSize"
          @keyup="saveSettings"
        />
      </div>
    </div>

    <div class="row mt-3">
      <div class="col-md-12">
        <h2>HttPlaceholder configuration</h2>

        <p>
          HttPlaceholder was started with the following settings. The settings
          are read-only and can only be set when starting the application.
        </p>
        <template v-for="item of config" :key="item.key">
          <div class="row setting">
            <div class="col-md-6">
              <strong>{{ item.key }}</strong>
            </div>
            <div class="col-md-6">{{ item.value }}</div>
          </div>
          <div class="divider mt-2 mb-2"></div>
        </template>
      </div>
    </div>

    <div class="row mt-3">
      <div class="col-md-12">
        <h2>Metadata</h2>
      </div>
      <div class="row">
        <div class="col-md-2"><strong>Version</strong></div>
      </div>
      <div class="row">
        <div class="col-md-2">{{ metadata.version }}</div>
      </div>
      <div class="row mt-2">
        <div class="col-md-2"><strong>Runtime</strong></div>
      </div>
      <div class="row">
        <div class="col-md-2">{{ metadata.runtimeVersion }}</div>
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
import { useMetadataStore } from "@/store/metadata";

export default defineComponent({
  name: "Settings",
  setup() {
    const generalStore = useSettingsStore();
    const configurationStore = useConfigurationStore();
    const metadataStore = useMetadataStore();

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
          (c) => c.key === storeResponsesKey,
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
    const metadata = computed(() => metadataStore.getMetadataState);

    // Lifecycle
    onMounted(async () => await loadConfig());

    return {
      settings,
      saveSettings,
      config,
      storeResponses,
      configDocsLink,
      metadata,
    };
  },
});
</script>

<style scoped lang="scss">
.setting > div {
  word-break: break-word;
}
</style>
