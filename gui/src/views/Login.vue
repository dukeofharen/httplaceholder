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
import { toastError } from "@/utils/toastUtil";
import { resources } from "@/shared/resources";
import { actionNames } from "@/store/storeConstants";
import { routeNames } from "@/router/routerConstants";

export default {
  name: "login",
  data() {
    return {
      username: "",
      password: ""
    };
  },
  methods: {
    async logIn() {
      if (this.username && this.password) {
        try {
          await this.$store.dispatch(actionNames.authenticate, {
            username: this.username,
            password: this.password
          });
          this.$router.push({ name: routeNames.requests });
        } catch (e) {
          if (e.response.status === 401) {
            toastError(resources.credentialsIncorrect);
          }
        }
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
