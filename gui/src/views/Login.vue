<template>
  <div class="login">
    <h1>Log in</h1>
    <div class="input-group mb-3">
      <div class="input-group-prepend">
        <span class="input-group-text fa fa-user"></span>
      </div>
      <input type="text" class="form-control" placeholder="Username" v-model="username" />
    </div>
    <div class="input-group mb-3">
      <div class="input-group-prepend">
        <span class="input-group-text fa fa-key"></span>
      </div>
      <input type="password" class="form-control" placeholder="Password" v-model="password" />
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

export default {
  name: "login",
  data() {
    return {
      username: "",
      password: ""
    };
  },
  methods: {
    logIn() {
      authenticate(this.username, this.password, response => {
        this.$router.push({ name: "requests" });
      }, error => {
        if(error.response.status === 401) {
          toastr.error(resources.credentialsIncorrect);
        } else {
          toastr.error(resources.somethingWentWrongServer);
        }
      })
    }
  }
};
</script>

<style scoped>
.input-group-text {
  width: 45px;
}
</style>