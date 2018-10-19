<template>
  <div class="requests">
    <h1>Requests</h1>
    <div class="row">
      <div class="col-md-10">
        <div class="input-group">
          <input type="text" class="form-control" placeholder="Filter on stub ID or URL..." v-model="searchTerm" />
          <span class="input-group-append">
            <a class="btn btn-outline-secondary" type="button" title="Clear input" v-on:click="clearInput"><span class="fa fa-eraser">&nbsp;</span></a>
          </span>
        </div>
      </div>
      <div class="col-md-1">
        <a class="btn btn-primary" v-on:click="deleteAllRequests" title="Delete all requests"><span class="fa fa-trash">&nbsp;</span></a>
      </div>
      <div class="col-md-1">
        <a class="btn btn-success" v-on:click="getRequests" title="Refresh"><span class="fa fa-refresh">&nbsp;</span></a>
      </div>
    </div>
    <Request v-bind:request="request" v-for="request in filteredRequests" :key="request.correlationId"></Request>
  </div>
</template>

<script>
import {
  shouldAuthenticate,
  logicGetRequests,
  logicDeleteAllRequests
} from "@/data/dataLogic";
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
    shouldAuthenticate(result => {
      if (!result) {
        this.getRequests();
      } else {
        this.$router.push({ name: "login" });
      }
    });
  },
  methods: {
    search(newValue) {
      if (!newValue) {
        this.filteredRequests = this.requests;
      } else {
        this.filteredRequests = this.requests.filter(
          r =>
            r.executingStubId && r.executingStubId.includes(newValue) ||
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
      logicDeleteAllRequests()
        .then(response => {
          toastr.success(resources.requestsDeletedSuccessfully);
          this.getRequests();
        })
        .catch(error => {
          toastr.error(resources.somethingWentWrongServer);
        });
    },
    getRequests() {
      logicGetRequests()
        .then(response => {
          this.requests = response.data;
          this.filteredRequests = response.data;
          this.handleUrlSearch();
        })
        .catch(error => {
          toastr.error(resources.somethingWentWrongServer);
        });
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
</style>