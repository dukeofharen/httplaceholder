<template>
  <v-row v-on:keyup.enter="logIn">
    <v-col>
      <v-card class="elevation-12">
        <v-card-text>
          <v-form>
            <v-text-field
              label="Login"
              name="login"
              prepend-icon="mdi-account"
              type="text"
              v-model="username"
            ></v-text-field>
            <v-text-field
              id="password"
              label="Password"
              name="password"
              prepend-icon="mdi-lock"
              type="password"
              v-model="password"
            ></v-text-field>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-btn color="primary" @click="logIn">Login</v-btn>
        </v-card-actions>
      </v-card>
    </v-col>
  </v-row>
</template>

<script>
import Request from "@/components/Request";
import { authenticateResults } from "@/shared/constants";

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
      if (this.username && this.password) {
        this.$store.dispatch("authenticate", {
          username: this.username,
          password: this.password
        });
      }
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
