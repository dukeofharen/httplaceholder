<template>
  <div class="download-stubs">
    <h1>Download all stubs</h1>
    <p>This page displays all stubs currently present in HttPlaceholder. You can copy this string and put it in a .yml file on your PC for local development.</p>
    <div class="input-group col-md-12" v-if="tenantNames.length > 0">
      <select v-model="selectedTenantName" class="form-control tenant-list">
        <option selected="selected" value>Select stub tenant / category name...</option>
        <option
          v-for="tenantName in tenantNames"
          v-bind:key="tenantName"
          v-bind:value="tenantName"
        >{{tenantName}}</option>
      </select>
    </div>
    <div class="input-group col-md-12">
      <textarea class="form-control" v-model="downloadString"></textarea>
    </div>
    <div class="col-md-12">
      <a v-on:click="downloadStubs" class="btn btn-primary">Download</a>
    </div>
  </div>
</template>

<script>
import yaml from "js-yaml";
import { resources } from "@/resources";
import { downloadBlob } from "@/functions/downloadHelper";

export default {
  name: "addStub",
  created() {
    this.$store.dispatch("getStubs");
    this.$store.dispatch("getTenantNames");
  },
  data() {
    return {
      downloadString: "",
      selectedTenantName: "",
      filteredStubs: []
    };
  },
  methods: {
    downloadStubs() {
      downloadBlob("stubs.yml", this.downloadString);
    }
  },
  computed: {
    stubs() {
      return this.$store.getters.getStubs;
    },
    tenantNames() {
      return this.$store.getters.getTenantNames;
    }
  },
  watch: {
    stubs(newStubs) {
      this.filteredStubs = newStubs;
    },
    filteredStubs(newStubs) {
      let stubsForDownload = newStubs.map(fullStub => {
        return fullStub.stub;
      });
      this.downloadString =
        resources.downloadStubsHeader + "\n" + yaml.dump(stubsForDownload);
    },
    selectedTenantName(val) {
      if (!val) {
        this.filteredStubs = this.stubs;
      } else {
        this.filteredStubs = this.stubs.filter(stub => stub.stub.tenant === val);
      }
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
.tenant-list {
  margin-bottom: 10px;
}
</style>