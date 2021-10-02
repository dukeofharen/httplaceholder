<template>
  <div class="container-fluid">
    <div class="row flex-nowrap">
      <Sidebar />
      <div class="col py-3">
        <router-view />
      </div>
    </div>
  </div>
</template>

<script>
import Sidebar from "@/components/Sidebar";
import { useStore } from "vuex";
import { computed, onMounted, watch } from "vue";
import { getDarkThemeEnabled } from "@/utils/session";

export default {
  components: { Sidebar },
  setup() {
    const store = useStore();

    // Computed
    const darkTheme = computed(() => store.getters["general/getDarkTheme"]);

    // Watch
    watch(darkTheme, (darkTheme) => {
      const bodyElement = document.body;
      const className = "dark-theme";
      if (darkTheme) {
        bodyElement.classList.add(className);
      } else {
        bodyElement.classList.remove(className);
      }
    });

    // Lifecycle
    onMounted(() => {
      const darkThemeEnabled = getDarkThemeEnabled();
      if (darkThemeEnabled) {
        store.commit("general/storeDarkTheme", darkThemeEnabled);
      }
    });
  },
};
</script>

<style lang="scss">
body {
  margin: 0;
  padding: 0;
  font-family: "Roboto", sans-serif !important;
}
</style>
