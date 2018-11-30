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
import Stub from "@/components/Stub";

export default {
  name: "stubs",
  data() {
    return {
      filteredStubs: [],
      searchTerm: ""
    };
  },
  components: {
    Stub
  },
  created() {
    this.getStubs();
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
      this.$store.dispatch('getStubs')
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
  computed: {
    stubs () {
      return this.$store.getters.getStubs
    }
  },
  watch: {
    searchTerm(newValue, oldValue) {
      this.search(newValue);
    },
    $route() {
      this.handleUrlSearch();
    },
    stubs (newStubs) {
      this.filteredStubs = newStubs
      this.handleUrlSearch()
    }
  }
};
</script>

<style scoped>
.buttons > a {
  margin-right: 5px;
}
</style>