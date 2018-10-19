<template>
  <div class="stubs">
    <h1>Stubs</h1>
    <div class="row">
      <div class="col-md-10">
        <div class="input-group">
          <input type="text" class="form-control" placeholder="Filter on stub ID or tenant..." v-model="searchTerm" />
          <span class="input-group-append">
            <a class="btn btn-outline-secondary" type="button" title="Clear input" v-on:click="clearInput"><span class="fa fa-eraser">&nbsp;</span></a>
          </span>
        </div>
      </div>
      <div class="col-md-2 buttons">
        <a class="btn btn-success" v-on:click="getStubs" title="Refresh"><span class="fa fa-refresh">&nbsp;</span></a>
        <a class="btn btn-success" v-on:click="addStub" title="Add new stub(s)"><span class="fa fa-plus-circle">&nbsp;</span></a>
        <a class="btn btn-success" v-on:click="downloadStubs" title="Download all stubs"><span class="fa fa-cloud-download">&nbsp;</span></a>
      </div>
    </div>
    <Stub v-bind:stub="stub" v-for="stub in filteredStubs" :key="stub.id"></Stub>
  </div>
</template>

<script>
import { shouldAuthenticate, logicGetStubs } from "@/data/dataLogic";
import Stub from "@/components/Stub";
import resources from "@/resources";
import toastr from "toastr";

export default {
  name: "stubs",
  data() {
    return {
      stubs: [],
      filteredStubs: [],
      searchTerm: ""
    };
  },
  components: {
    Stub
  },
  created() {
    shouldAuthenticate(result => {
      if (!result) {
        this.getStubs();
      } else {
        this.$router.push({ name: "login" });
      }
    });
  },
  methods: {
    search(newValue) {
      if (!newValue) {
        this.filteredStubs = this.stubs;
      } else {
        this.filteredStubs = this.stubs.filter(r => {
          return r.id.includes(newValue) ||
                 r.tenant && r.tenant.includes(newValue);
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
      logicGetStubs()
        .then(response => {
          this.stubs = response.data;
          this.filteredStubs = response.data;
          this.handleUrlSearch();
        })
        .catch(error => {
          toastr.error(resources.somethingWentWrongServer);
        });
    },
    addStub() {
      this.$router.push({ name: "addStub" });
    },
    downloadStubs() {
      this.$router.push({ name: "downloadStubs" });
    },
    clearInput() {
      this.searchTerm = "";
    }
  },
  watch: {
    searchTerm(newValue, oldValue) {
      this.search(newValue);
    },
    $route(from, to) {
      this.handleUrlSearch();
    }
  }
};
</script>

<style scoped>
.buttons > a {
  margin-right: 5px;
}
</style>