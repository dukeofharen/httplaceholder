<template>
  <div class="login" v-on:keyup.enter="logIn">
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
import Request from "@/components/Request";
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
    lastAuthenticateResult(result) {
      if (result == authenticateResults.OK) {
        this.$router.push({ name: "requests" });
      }
    }
  }
};
</script>

<style scoped>
.input-group input {
  border-left: 0;
}
</style>