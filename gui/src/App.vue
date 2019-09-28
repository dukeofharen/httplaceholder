<template>
  <v-app id="keep">
    <v-app-bar app clipped-left color="amber">
      <v-app-bar-nav-icon @click="drawer = !drawer"></v-app-bar-nav-icon>
      <span class="title ml-3 mr-5">HttPlaceholder</span>
      <div class="flex-grow-1"></div>
    </v-app-bar>

    <v-navigation-drawer v-model="drawer" app clipped>
      <v-list dense>
        <template>
          <v-list-item @click="toRequests" v-if="authenticated">
            <v-list-item-action>
              <v-icon>mdi-google-chrome</v-icon>
            </v-list-item-action>
            <v-list-item-title class="grey--text">Requests</v-list-item-title>
          </v-list-item>
          <v-divider dark class="my-4" v-if="authenticated"></v-divider>
          <v-list-item @click="toStubs" v-if="authenticated">
            <v-list-item-action>
              <v-icon>mdi-controller-classic</v-icon>
            </v-list-item-action>
            <v-list-item-title class="grey--text">Stubs</v-list-item-title>
          </v-list-item>
          <v-list-item @click="toAddStub" v-if="authenticated">
            <v-list-item-action>
              <v-icon>mdi-plus</v-icon>
            </v-list-item-action>
            <v-list-item-title class="grey--text">Add stubs</v-list-item-title>
          </v-list-item>
          <v-list-item @click="toDownloadStubs" v-if="authenticated">
            <v-list-item-action>
              <v-icon>mdi-download</v-icon>
            </v-list-item-action>
            <v-list-item-title class="grey--text">Download stubs</v-list-item-title>
          </v-list-item>
          <v-divider dark class="my-4" v-if="authenticated && authenticationRequired"></v-divider>
          <v-list-item @click="logout" v-if="authenticated && authenticationRequired">
            <v-list-item-action>
              <v-icon>mdi-exit-to-app</v-icon>
            </v-list-item-action>
            <v-list-item-title class="grey--text">Log out</v-list-item-title>
          </v-list-item>
        </template>
      </v-list>
    </v-navigation-drawer>

    <v-content>
      <v-container fluid class="lighten-4">
        <router-view></router-view>
      </v-container>
    </v-content>
  </v-app>
</template>

<script>
import { messageTypes, authenticateResults } from "@/constants";
import toastr from "toastr";

export default {
  name: "app",
  created() {
    // this.$vuetify.theme.dark = true;
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
      drawer: true
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
    }
  },
  methods: {
    logout() {
      sessionStorage.removeItem("userToken");
      this.$store.commit("storeAuthenticated", false);
      this.$router.push({ name: "login" });
    },
    toRequests() {
      this.$router.push({ name: "requests" });
    },
    toStubs() {
      this.$router.push({ name: "stubs" });
    },
    toAddStub() {
      this.$router.push({ name: "addStub" });
    },
    toDownloadStubs() {
      this.$router.push({ name: "downloadStubs" });
    }
  },
  watch: {
    authenticated(isAuthenticated) {
      if (!isAuthenticated) {
        this.$router.push({ name: "login" });
      }
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
