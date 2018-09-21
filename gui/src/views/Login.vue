<template>
  <div class="login">
    <h1>Log in</h1>
  </div>
</template>

<script>
import { getRequests, deleteAllRequests } from "@/data/serviceAgent";
import Request from "@/components/Request";
import resources from "@/resources";
import toastr from "toastr";

export default {
  name: "login",
  data() {
    return {
    };
  },
  components: {
  },
  created() {
  },
  methods: {
  },
  watch: {
  }
};
</script>

<style scoped>
</style>