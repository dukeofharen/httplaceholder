<template>
  <div class="stubs">
    <h1>Stubs</h1>
    <div class="row">
      <div class="col-md-7">
        <div class="input-group">
          <input
            type="text"
            class="form-control"
            placeholder="Filter on stub ID or tenant..."
            v-model="searchTerm"
          />
          <span class="input-group-append">
            <a
              class="btn btn-outline-secondary"
              type="button"
              title="Clear input"
              v-on:click="clearInput"
            >
              <span class="fa fa-eraser">&nbsp;</span>
            </a>
          </span>
        </div>
      </div>
      <div class="col-md-5 buttons">
        <a class="btn btn-danger" v-on:click="deleteAllStubs" title="Delete all stubs">
          <span class="fa fa-trash">&nbsp;</span>
        </a>
        <a class="btn btn-success" v-on:click="getStubs" title="Refresh">
          <span class="fa fa-refresh">&nbsp;</span>
        </a>
        <router-link to="/downloadStubs" class="btn btn-success" title="Download stubs">
          <span class="fa fa-cloud-download">&nbsp;</span>
        </router-link>
        <router-link to="/addStub" class="btn btn-success" title="Add stubs">
          <span class="fa fa-plus-circle">&nbsp;</span>
        </router-link>
      </div>
    </div>
    <div class="row">
      <div class="input-group col-md-12" v-if="tenantNames.length > 0">
        <select v-model="selectedTenantName" class="form-control tenant-list">
          <option
            selected="selected"
            value
          >Select stub tenant / category name for the stubs you would like to download...</option>
          <option
            v-for="tenantName in tenantNames"
            v-bind:key="tenantName"
            v-bind:value="tenantName"
          >{{tenantName}}</option>
        </select>
      </div>
    </div>
    <Stub v-bind:fullStub="stub" v-for="stub in filteredStubs" :key="stub.id"></Stub>
  </div>
</template>

<script>
import Stub from "@/components/Stub";
import { resources } from "@/resources";

export default {
  name: "stubs",
  data() {
    return {
      filteredStubs: [],
      selectedTenantName: "",
      searchTerm: ""
    };
  },
  components: {
    Stub
  },
  created() {
    this.getStubs();
    this.getTenantNames();
  },
  methods: {
    search(newValue) {
      if (!newValue) {
        this.filteredStubs = this.stubs;
      } else {
        this.filteredStubs = this.stubs.filter(r => {
          return (
            r.stub.id.includes(newValue) ||
            (r.stub.tenant && r.stub.tenant.includes(newValue))
          );
        });
      }
    },
    handleUrlSearch() {
      let term = this.$route.query.searchTerm;
      if (term) {
        this.searchTerm = term;
      }
    },
    getStubs() {
      this.$store.dispatch("getStubs");
    },
    getTenantNames() { 
      this.$store.dispatch("getTenantNames");
    },
    addStub() {
      this.$router.push({ name: "addStub" });
    },
    downloadStubs() {
      this.$router.push({ name: "downloadStubs" });
    },
    clearInput() {
      this.searchTerm = "";
    },
    deleteAllStubs() {
      this.$dialog.confirm(resources.areYouSure).then(() => {
        this.$store.dispatch("deleteAllStubs");
      });
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
    searchTerm(newValue, oldValue) {
      this.search(newValue);
    },
    $route() {
      this.handleUrlSearch();
    },
    stubs(newStubs) {
      this.filteredStubs = newStubs;
      this.handleUrlSearch();
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
.tenant-list {
  margin-top: 10px;
}
</style>