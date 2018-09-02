<template>
  <div class="requests">
    <h1>Requests</h1>
    <input type="text" class="form-control" placeholder="Filter on stub ID or URL..." v-model="searchTerm" />
    <Request v-bind:request="request" v-for="request in filteredRequests" :key="request.correlationId"></Request>
  </div>
</template>

<script>
import { getRequests } from "@/data/serviceAgent";
import Request from "@/components/Request";

export default {
  name: "requests",
  data() {
    return {
      requests: [],
      filteredRequests: [],
      searchTerm: ""
    };
  },
  components: {
    Request
  },
  created() {
    getRequests()
      .then(response => {
        this.requests = response.data;
        this.filteredRequests = response.data;
        this.handleUrlSearch();
      })
      .catch(error => {
        // TODO show error message
        console.log(error);
      });
  },
  methods: {
    search(newValue) {
      if (!newValue) {
        this.filteredRequests = this.requests;
      } else {
        this.filteredRequests = this.requests.filter(
          r =>
            r.executingStubId.includes(newValue) ||
            r.requestParameters.url.includes(newValue)
        );
      }
    },
    handleUrlSearch(){
      let term = this.$route.query.searchTerm;
      if(term) {
        this.searchTerm = term;
      }
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
.request {
  background-color: #f8f9fa;
  margin: 10px;
  padding: 10px;
}
</style>