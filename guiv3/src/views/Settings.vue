<template>
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
</template>

<script>
import { ref } from "vue";
import { useStore } from "vuex";

export default {
  name: "Settings",
  setup() {
    const store = useStore();

    // Data
    const settings = ref(store.getters["general/getSettings"]);

    // Methods
    const saveSettings = () => {
      store.commit("general/storeSettings", settings.value);
    };

    return { settings, saveSettings };
  },
};
</script>

<style scoped></style>
