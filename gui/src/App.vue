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

export default {
  name: "app",
  components: { NavDrawer, MenuBar },
  beforeMount() {
    this.setTheme();
  },
  async created() {
    this.metadata = await this.$store.dispatch("metadata/getMetadata");
    document.title = `HttPlaceholder - v${this.metadata.version}`;

    if (!this.authenticated) {
      await this.$store.dispatch("users/ensureAuthenticated");
    }
  },
  data() {
    return {
      routeNames
    };
  },
  computed: {
    authenticated() {
      return this.$store.getters["users/getAuthenticated"];
    },
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
