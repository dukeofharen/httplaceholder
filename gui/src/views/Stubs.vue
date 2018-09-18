<template>
  <div class="stubs">
    <h1>Stubs</h1>
    <div class="row">
      <div class="col-md-11">
        <input type="text" class="form-control" placeholder="Filter on stub ID..." v-model="searchTerm" />
      </div>
      <div class="col-md-1">
        <a class="btn btn-success" v-on:click="getStubs"><span class="fa fa-refresh">&nbsp;</span></a>
      </div>
    </div>
    <Stub v-bind:stub="stub" v-for="stub in filteredStubs" :key="stub.id"></Stub>
  </div>
</template>

<script>
import { getStubs } from "@/data/serviceAgent";
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
    this.getStubs()
  },
  methods: {
    search(newValue) {
      if (!newValue) {
        this.filteredStubs = this.stubs;
      } else {
        this.filteredStubs = this.stubs.filter(r => r.id.includes(newValue));
      }
    },
    handleUrlSearch() {
      let term = this.$route.query.searchTerm;
      if (term) {
        this.searchTerm = term;
      }
    },
    getStubs() {
      getStubs()
      .then(response => {
        this.stubs = response.data;
        this.filteredStubs = response.data;
        this.handleUrlSearch();
      })
      .catch(error => {
        toastr.error(resources.somethingWentWrongServer);
      });
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
</style>