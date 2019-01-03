<template>
  <div class="download-stubs">
    <h1>Download all stubs</h1>
    <p>This page displays all stubs currently present in HttPlaceholder. You can copy this string and put it in a .yml file on your PC for local development.</p>
    <div class="input-group col-md-12">
      <textarea class="form-control">{{downloadString}}</textarea>
    </div>
    <div class="col-md-12">
      <a :href="downloadUrl" download="stubs.yml" class="btn btn-primary">Download</a>
    </div>
  </div>
</template>

<script>
import yaml from "js-yaml";
import resources from "@/resources";

export default {
  name: "addStub",
  created() {
    this.$store.dispatch("getStubs");
  },
  data() {
    return {
      downloadString: "",
      downloadUrl: ""
    }
  },
  computed: {
    stubs() {
      return this.$store.getters.getStubs;
    }
  },
  watch: {
    stubs(newStubs) {
      let stubsForDownload = newStubs.map(fullStub => {
        return fullStub.stub
      })
      this.downloadString = resources.downloadStubsHeader + "\n" + yaml.dump(stubsForDownload);
      this.downloadUrl = `data:application/octet-stream;charset=utf-8;base64,${btoa(this.downloadString)}`
    }
  }
};
</script>

<style scoped>
.download-stubs {
  text-align: left;
}

.input-group textarea {
  height: 200px;
  margin-bottom: 10px;
}
</style>