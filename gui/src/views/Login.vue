<template>
  <div class="login">
    <h1>Log in</h1>
    <div class="input-group mb-3">
      <div class="input-group-prepend">
        <span class="input-group-text fa fa-user"></span>
      </div>
      <input type="text" class="form-control" placeholder="Username" v-model="username">
    </div>
    <div class="input-group mb-3">
      <div class="input-group-prepend">
        <span class="input-group-text fa fa-key"></span>
      </div>
      <input type="password" class="form-control" placeholder="Password" v-model="password">
    </div>
    <div class="input-group">
      <a class="btn btn-success" v-on:click="logIn">Log in</a>
    </div>
  </div>
</template>

<script>
import { authenticate } from "@/data/dataLogic";
import Request from "@/components/Request";
import resources from "@/resources";
import toastr from "toastr";
import { authenticateResults } from "@/constants";

export default {
  name: "login",
  data() {
    return {
      username: "",
      password: ""
    };
  },
  computed: {
    lastAuthenticateResult() {
      return this.$store.getters.getLastAuthenticateResult;
    },
    userToken () {
      return this.$store.getters.getUserToken;
    }
  },
  methods: {
    logIn() {
      this.$store.dispatch("authenticate", {
        username: this.username,
        password: this.password
      });
    }
  },
  watch: {
    userToken(token) {
      console.log(token)
      console.log(this.lastAuthenticateResult)
      if (this.lastAuthenticateResult == authenticateResults.INVALID_CREDENTIALS) {
        toastr.error(resources.credentialsIncorrect);
      } else if (this.lastAuthenticateResult == authenticateResults.INTERNAL_SERVER_ERROR) {
        toastr.error(resources.somethingWentWrongServer);
      } else if (this.lastAuthenticateResult == authenticateResults.OK) {
        this.$router.push({ name: "requests" });
      }
    }
  }
};
</script>

<style scoped>
.input-group-text {
  width: 45px;
}
</style>