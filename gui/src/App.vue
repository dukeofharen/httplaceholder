<template>
  <div id="app">
    <nav class="navbar navbar-expand-lg">
      <a class="navbar-brand" href="#">HttPlaceholder</a>
      <button
        class="navbar-toggler"
        type="button"
        data-toggle="collapse"
        data-target="#navbarSupportedContent"
        aria-controls="navbarSupportedContent"
        aria-expanded="false"
        aria-label="Toggle navigation"
      >
        <span class="navbar-toggler-icon"></span>
      </button>

      <div class="collapse navbar-collapse" id="navbarSupportedContent">
        <ul class="navbar-nav mr-auto">
          <li class="nav-item" v-if="authenticated">
            <router-link to="/requests" class="nav-link">Requests</router-link>
          </li>
          <li class="nav-item dropdown" v-if="authenticated">
            <a
              class="nav-link dropdown-toggle"
              href="#"
              id="stubsDropdown"
              data-toggle="dropdown"
              aria-haspopup="true"
              aria-expanded="false"
            >Stubs</a>
            <div class="dropdown-menu" aria-labelledby="stubsDropdown">
              <router-link to="/stubs" class="dropdown-item">Stubs</router-link>
              <router-link to="/addStub" class="dropdown-item">Add stub</router-link>
              <router-link to="/downloadStubs" class="dropdown-item">Download stubs</router-link>
            </div>
          </li>
          <li class="nav-item dropdown">
            <a
              class="nav-link dropdown-toggle"
              href="#"
              id="themesDropdown"
              data-toggle="dropdown"
              aria-haspopup="true"
              aria-expanded="false"
            >Themes</a>
            <div class="dropdown-menu" aria-labelledby="themesDropdown">
              <a
                href="#"
                v-on:click="changeThemeClick(theme)"
                v-for="theme in themes"
                v-bind:key="theme.name"
                class="dropdown-item"
              >{{theme.name}}</a>
            </div>
          </li>
          <li class="nav-item" v-if="authenticationRequired && authenticated">
            <a href="#" v-on:click="logout()" class="nav-link">Log out</a>
          </li>
        </ul>
      </div>
    </nav>
    <div class="container application">
      <router-view></router-view>
    </div>
  </div>
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
