<template>
  <v-app id="keep">
    <v-app-bar app clipped-left color="amber">
      <v-app-bar-nav-icon @click="drawer = !drawer"></v-app-bar-nav-icon>
      <span class="title ml-3 mr-5">HttPlaceholder</span>
      <div class="flex-grow-1"></div>
    </v-app-bar>

    <v-navigation-drawer v-model="drawer" app clipped color="grey lighten-4">
      <v-list dense class="grey lighten-4">
        <template>
          <v-list-item @click="toRequests">
            <v-list-item-action>
              <v-icon>mdi-google-chrome</v-icon>
            </v-list-item-action>
            <v-list-item-title class="grey--text">
              Requests
            </v-list-item-title>
          </v-list-item>
          <v-divider
            :key="i"
            dark
            class="my-4"
          ></v-divider>
          <v-list-item @click="toStubs">
            <v-list-item-action>
              <v-icon>mdi-controller-classic</v-icon>
            </v-list-item-action>
            <v-list-item-title class="grey--text">
              Stubs
            </v-list-item-title>
          </v-list-item>
          <v-list-item @click="toAddStub">
            <v-list-item-action>
              <v-icon>mdi-plus</v-icon>
            </v-list-item-action>
            <v-list-item-title class="grey--text">
              Add stubs
            </v-list-item-title>
          </v-list-item>
          <v-list-item @click="toDownloadStubs">
            <v-list-item-action>
              <v-icon>mdi-download</v-icon>
            </v-list-item-action>
            <v-list-item-title class="grey--text">
              Download stubs
            </v-list-item-title>
          </v-list-item>
          <v-divider
            :key="i"
            dark
            class="my-4"
          ></v-divider>
          <v-list-item @click="">
            <v-list-item-action>
              <v-icon>mdi-cogs</v-icon>
            </v-list-item-action>
            <v-list-item-title class="grey--text">
              Settings
            </v-list-item-title>
          </v-list-item>
          <!-- TODO logout -->
        </template>
      </v-list>
    </v-navigation-drawer>

    <v-content>
      <v-container fluid class="grey lighten-4 fill-height">
        <v-row justify="center" align="center">
          <v-col class="shrink">
            <router-view></router-view>
          </v-col>
        </v-row>
      </v-container>
    </v-content>
  </v-app>
</template>

<script>
import { messageTypes, themes } from "@/constants";
import toastr from "toastr";

export default {
  name: "app",
  created() {
    let themeText = sessionStorage.theme;
    if (themeText) {
      let theme = JSON.parse(themeText);
      this.$store.commit("storeTheme", theme);
    }

    this.changeTheme();
    let token = sessionStorage.userToken;
    if (token) {
      this.$store.commit("storeUserToken", token);
      this.$store.commit("storeAuthenticated", true);
    } else {
      this.$store.dispatch("ensureAuthenticated");
    }

    this.$store.dispatch("refreshMetadata");
  },
  data() {
    return {
      themes: themes
    };
  },
  computed: {
    metadata() {
      return this.$store.getters.getMetadata;
    },
    authenticated() {
      return this.$store.getters.getAuthenticated;
    },
    toast() {
      return this.$store.getters.getToast;
    },
    userToken() {
      return this.$store.getters.getUserToken;
    },
    authenticationRequired() {
      return this.$store.getters.getAuthenticationRequired;
    },
    theme() {
      return this.$store.getters.getTheme;
    }
  },
  methods: {
    logout() {
      sessionStorage.removeItem("userToken");
      this.$store.commit("storeAuthenticated", false);
      this.$router.push({ name: "login" });
    },
    changeThemeClick(theme) {
      this.$store.commit("storeTheme", theme);
      sessionStorage.theme = JSON.stringify(theme);
    },
    changeTheme(oldTheme) {
      if (oldTheme) {
        document.body.classList.remove(oldTheme.className);
      }

      document.body.classList.add(this.theme.className);
    },
    toRequests() {
      this.$router.push({name: "requests"});
    },
    toStubs() {
      this.$router.push({name: "stubs"});
    },
    toAddStub() {
      this.$router.push({name: "addStub"});
    },
    toDownloadStubs() {
      this.$router.push({name: "downloadStubs"});
    }
  },
  watch: {
    authenticated(isAuthenticated) {
      if (!isAuthenticated) {
        this.$router.push({ name: "login" });
      }
    },
    theme(newTheme, oldTheme) {
      this.changeTheme(oldTheme);
    },
    toast(newToast) {
      switch (newToast.type) {
        case messageTypes.SUCCESS:
          toastr.success(newToast.message);
          break;
        case messageTypes.WARNING:
          toastr.warning(newToast.message);
          break;
        case messageTypes.ERROR:
          toastr.error(newToast.message);
          break;
        default:
        case messageTypes.INFO:
          toastr.info(newToast.message);
          break;
      }
    },
    userToken(newToken) {
      if (!newToken) {
        sessionStorage.removeItem("userToken");
      } else {
        sessionStorage.userToken = newToken;
      }
    },
    metadata() {
      // Add version to title tag
      document.title = `HttPlaceholder - v${this.metadata.version}`;
    }
  }
};
</script>

<style>
</style>
