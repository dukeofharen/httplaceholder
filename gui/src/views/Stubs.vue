<template>
  <v-row>
    <v-col>
      <h1>Stubs</h1>
      <v-row>
        <v-col cols="6">
          <v-text-field v-model="searchTerm" placeholder="Filter on stub ID or tenant..." clearable></v-text-field>
          <v-select
            :items="tenantNames"
            placeholder="Select stub tenant / category name for the stubs you would like to see the stubs for..."
            v-model="selectedTenantName"
            clearable
          ></v-select>
        </v-col>
        <v-expansion-panels>
          <Stub v-bind:fullStub="stub" v-for="stub in filteredStubs" :key="stub.id"></Stub>
        </v-expansion-panels>
      </v-row>
    </v-col>
  </v-row>
  <!-- <div class="stubs">
    <h1>Stubs</h1>
    <div class="row">
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
    
  </div>-->
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

      let tenant = this.$route.query.stubTenant;
      if (tenant) {
        this.selectedTenantName = tenant;
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
        this.filteredStubs = this.stubs.filter(
          stub => stub.stub.tenant === val
        );
      }
    }
  }
};
</script>

<style scoped>
.buttons > button {
  margin-right: 10px;
  margin-top: 10px;
}
</style>