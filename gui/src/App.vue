<template>
  <v-app id="keep">
    <MenuBar />
    <NavDrawer />

    <v-main>
      <v-container fluid class="lighten-4">
        <router-view :key="$route.fullPath"></router-view>
      </v-container>
    </v-main>
  </v-app>
</template>

<script>
import { routeNames } from "@/router/routerConstants";
import { getDarkThemeEnabled } from "@/utils/sessionUtil";
import MenuBar from "@/components/navigation/MenuBar";
import NavDrawer from "@/components/navigation/NavDrawer";
import router from "@/router";

export default {
  name: "app",
  components: { NavDrawer, MenuBar },
  beforeMount() {
    this.setTheme();
  },
  async created() {
    this.metadata = await this.$store.dispatch("metadata/getMetadata");
    document.title = `HttPlaceholder - v${this.metadata.version}`;

    const authEnabled = await this.$store.dispatch(
      "metadata/checkAuthenticationIsEnabled"
    );
    if (!this.$store.getters["users/getAuthenticated"] && authEnabled) {
      await router.push({ name: routeNames.login });
    }
  },
  data() {
    return {
      routeNames
    };
  },
  computed: {
    darkTheme() {
      return this.$store.getters["general/getDarkTheme"];
    }
  },
  methods: {
    setTheme() {
      const darkThemeEnabled = getDarkThemeEnabled();
      if (darkThemeEnabled) {
        this.$store.commit("general/storeDarkTheme", darkThemeEnabled);
      }
    },
    flipDrawerIsOpen() {
      this.$store.commit("general/flipDrawerIsOpen");
    }
  },
  watch: {
    darkTheme(darkTheme) {
      this.$vuetify.theme.dark = darkTheme;
    }
  }
};
</script>
