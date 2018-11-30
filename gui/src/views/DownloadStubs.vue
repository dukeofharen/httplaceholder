<template>
  <div class="download-stubs">
    <h1>Download all stubs</h1>
    <p>
        This page displays all stubs currently present in HttPlaceholder. You can copy this string and put it in a .yml file on your PC for local development.
    </p>
    <div class="input-group">
      <textarea class="form-control">{{storeStubsDownloadString}}</textarea>
    </div>
  </div>
</template>

<script>
import { shouldAuthenticate, logicGetStubs } from "@/data/dataLogic";
import resources from "@/resources";
import toastr from "toastr";
import yaml from "js-yaml";

export default {
  name: "addStub",
  created() {
    this.$store.dispatch('getStubsAsYaml')
  },
  computed: {
    storeStubsDownloadString() {
      return this.$store.getters.stubsDownloadString
    }
  }
};
</script>

<style scoped>
.download-stubs{ 
    text-align: left;
}

.input-group textarea {
  height: 200px;
  margin-bottom: 10px;
}
</style>