<template>
  <div class="container-fluid">
    <div class="row flex-nowrap">
      <Sidebar />
      <div class="col-md-10 col-10 col-xl-10 col-lg-10 col-sm-9 py-3 main-body">
        <router-view v-slot="{ Component }">
          <transition name="fade" mode="out-in">
            <component :is="Component" />
          </transition>
        </router-view>
      </div>
    </div>
  </div>
</template>

<script>
import Sidebar from "@/components/Sidebar.vue";
import { computed, defineComponent, onMounted, watch } from "vue";
import { useRouter } from "vue-router";
import { useUsersStore } from "@/store/users";
import { useMetadataStore } from "@/store/metadata";
import { useGeneralStore } from "@/store/general";

export default defineComponent({
  components: { Sidebar },
  setup() {
    const userStore = useUsersStore();
    const metadataStore = useMetadataStore();
    const generalStore = useGeneralStore();
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
    const darkTheme = computed(() => generalStore.getDarkTheme);

    // Watch
    watch(darkTheme, (darkTheme) => setDarkTheme(darkTheme));

    // Lifecycle
    onMounted(async () => {
      const darkThemeEnabled = darkTheme.value;
      setDarkTheme(darkThemeEnabled);
      metadataStore
        .getMetadata()
        .then((m) => (document.title = `HttPlaceholder - v${m.version}`));

      const authEnabled = await metadataStore.checkAuthenticationIsEnabled();
      if (!userStore.getAuthenticated && authEnabled) {
        await router.push({ name: "Login" });
      }
    });
  },
});
</script>

<style lang="scss">
body {
  margin: 0;
  padding: 0;
  font-family: "Roboto Mono", sans-serif !important;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.1s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
