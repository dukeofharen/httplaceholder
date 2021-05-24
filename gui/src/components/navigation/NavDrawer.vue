<template>
  <v-navigation-drawer v-model="drawer" app clipped>
    <v-list dense>
      <template>
        <v-list-item v-if="authenticated" :to="{ name: routeNames.requests }">
          <v-list-item-action>
            <v-icon>mdi-google-chrome</v-icon>
          </v-list-item-action>
          <v-list-item-title class="grey--text">Requests</v-list-item-title>
        </v-list-item>
        <v-divider dark class="my-4" v-if="authenticated"></v-divider>
        <v-list-item v-if="authenticated" :to="{ name: routeNames.stubs }">
          <v-list-item-action>
            <v-icon>mdi-controller-classic</v-icon>
          </v-list-item-action>
          <v-list-item-title class="grey--text">Stubs</v-list-item-title>
        </v-list-item>
        <v-list-item v-if="authenticated" :to="{ name: routeNames.stubForm }">
          <v-list-item-action>
            <v-icon>mdi-plus</v-icon>
          </v-list-item-action>
          <v-list-item-title class="grey--text">Add stubs</v-list-item-title>
        </v-list-item>
        <v-list-item v-if="authenticated" :to="{ name: routeNames.uploadStub }">
          <v-list-item-action>
            <v-icon>mdi-plus</v-icon>
          </v-list-item-action>
          <v-list-item-title class="grey--text"
            >Upload stubs
          </v-list-item-title>
        </v-list-item>
        <v-list-item
          v-if="authenticated"
          :to="{ name: routeNames.downloadStubs }"
        >
          <v-list-item-action>
            <v-icon>mdi-download</v-icon>
          </v-list-item-action>
          <v-list-item-title class="grey--text"
            >Download stubs
          </v-list-item-title>
        </v-list-item>
        <v-divider dark class="my-4" v-if="authenticated"></v-divider>
        <v-list-item v-if="authenticated" :to="{ name: routeNames.settings }">
          <v-list-item-action>
            <v-icon>mdi-cogs</v-icon>
          </v-list-item-action>
          <v-list-item-title class="grey--text">Settings</v-list-item-title>
        </v-list-item>
        <v-divider
          dark
          class="my-4"
          v-if="authenticated && authRequired"
        ></v-divider>
        <v-list-item @click="logout" v-if="authenticated && authRequired">
          <v-list-item-action>
            <v-icon>mdi-exit-to-app</v-icon>
          </v-list-item-action>
          <v-list-item-title class="grey--text">Log out</v-list-item-title>
        </v-list-item>
      </template>
    </v-list>
  </v-navigation-drawer>
</template>

<script>
import { routeNames } from "@/router/routerConstants";

export default {
  name: "NavDrawer",
  data() {
    return {
      routeNames
    };
  },
  computed: {
    authenticated() {
      return this.$store.getters["users/getAuthenticated"];
    },
    authRequired() {
      return this.$store.getters["users/getAuthRequired"];
    },
    drawer: {
      get() {
        return this.$store.getters["general/getDrawerIsOpen"];
      },
      set(value) {
        this.$store.commit("general/setDrawerState", value);
      }
    }
  },
  methods: {
    logout() {
      this.$store.commit("users/storeUserToken", null);
      this.$router.push({ name: routeNames.login });
    }
  }
};
</script>
