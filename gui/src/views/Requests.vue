<template>
  <div class="requests">
    <h1>Requests</h1>
    <div class="row">
      <div class="col-10">
        <input type="text" class="form-control" placeholder="Filter on stub ID or URL..." v-model="searchTerm" />
      </div>
      <div class="col-2">
        <a class="btn btn-primary" v-on:click="deleteAllRequests">Clear all requests</a>
      </div>
    </div>
    <Request v-bind:request="request" v-for="request in filteredRequests" :key="request.correlationId"></Request>
  </div>
</template>

<script>
import { getRequests, deleteAllRequests } from "@/data/serviceAgent";
import Request from "@/components/Request";
import resources from "@/resources";
import toastr from "toastr";

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
    this.getRequests()
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
    handleUrlSearch() {
      let term = this.$route.query.searchTerm;
      if (term) {
        this.searchTerm = term;
      }
    },
    deleteAllRequests() {
      deleteAllRequests()
        .then(response => {
          toastr.success(resources.requestsDeletedSuccessfully)
          this.getRequests()
        })
        .catch(error => {
          toastr.error(resources.somethingWentWrongServer)
        })
    },
    getRequests() {
      getRequests()
      .then(response => {
        this.requests = response.data;
        this.filteredRequests = response.data;
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