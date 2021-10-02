<template>
  <h1>Settings</h1>

  <div class="form-check">
    <input
      class="form-check-input"
      type="checkbox"
      id="darkTheme"
      v-model="darkMode"
      @change="darkModeChanged"
    />
    <label class="form-check-label" for="darkTheme">Dark theme</label>
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
    const darkMode = ref(store.getters["general/getDarkTheme"]);

    // Methods
    const darkModeChanged = () => {
      store.commit("general/storeDarkTheme", darkMode.value);
    };

    return { darkMode, darkModeChanged };
  },
};
</script>

<style scoped></style>
