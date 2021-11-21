<template>
  <div class="container-fluid">
    <div class="row flex-nowrap">
      <Sidebar />
      <div class="col py-3 main-body">
        <router-view />
      </div>
    </div>
  </div>
</template>

<script>
import Sidebar from "@/components/Sidebar";
import { useStore } from "vuex";
import { computed, onMounted, watch } from "vue";
import { useRouter } from "vue-router";

export default {
  components: { Sidebar },
  setup() {
    const store = useStore();
    const router = useRouter();

    // Functions
    const setDarkTheme = (darkTheme) => {
      const bodyElement = document.body;
      const darkName = "dark-theme";
      const lightName = "light-theme";
      if (darkTheme) {
        bodyElement.classList.remove(lightName);
        bodyElement.classList.add(darkName);
      } else {
        bodyElement.classList.remove(darkName);
        bodyElement.classList.add(lightName);
      }
    };

    // Computed
    const darkTheme = computed(() => store.getters["general/getDarkTheme"]);

    // Watch
    watch(darkTheme, (darkTheme) => setDarkTheme(darkTheme));

    // Lifecycle
    onMounted(async () => {
      const darkThemeEnabled = store.getters["general/getDarkTheme"];
      setDarkTheme(darkThemeEnabled);
      store
        .dispatch("metadata/getMetadata")
        .then((m) => (document.title = `HttPlaceholder - v${m.version}`));

      const authEnabled = await store.dispatch(
        "metadata/checkAuthenticationIsEnabled"
      );
      if (!store.getters["users/getAuthenticated"] && authEnabled) {
        await router.push({ name: "Login" });
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
