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
import { getDarkThemeEnabled } from "@/utils/session";
import { useRouter } from "vue-router";

export default {
  components: { Sidebar },
  setup() {
    const store = useStore();
    const router = useRouter();

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
    onMounted(async () => {
      const darkThemeEnabled = getDarkThemeEnabled();
      if (darkThemeEnabled) {
        store.commit("general/storeDarkTheme", darkThemeEnabled);
      }

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
