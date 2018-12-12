<template>
  <div id="app">
    <nav class="navbar navbar-expand-lg navbar-light bg-light">
      <a class="navbar-brand" href="#">HttPlaceholder {{metadata.version}}</a>
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

      <div class="collapse navbar-collapse" id="navbarSupportedContent" v-if="authenticated">
        <ul class="navbar-nav mr-auto">
          <li class="nav-item">
            <router-link to="/requests" class="nav-link">Requests</router-link>
          </li>
          <li class="nav-item dropdown">
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
          <li class="nav-item" v-if="authenticationRequired">
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
import { messageTypes } from "@/constants";
import toastr from "toastr";

export default {
  name: "app",
  created() {
    let token = sessionStorage.userToken;
    if (token) {
      this.$store.commit("storeUserToken", token);
      this.$store.commit("storeAuthenticated", true);
    } else {
      this.$store.dispatch("ensureAuthenticated");
    }

    this.$store.dispatch("refreshMetadata");
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
    }
  }
};
</script>

<style>
#app {
  font-family: "Avenir", Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}
#nav {
  padding: 30px;
}

#nav a {
  font-weight: bold;
  color: #2c3e50;
}

#nav a.router-link-exact-active {
  color: #42b983;
}
</style>
