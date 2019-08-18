<template>
  <div class="requests">
    <h1>Requests</h1>
    <div class="row">
      <div class="col-md-7">
        <div class="input-group">
          <input
            type="text"
            class="form-control"
            placeholder="Filter on stub ID or URL..."
            v-model="searchTerm"
          >
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
        <a class="btn btn-danger" v-on:click="deleteAllRequests" title="Delete all requests">
          <span class="fa fa-trash">&nbsp;</span>
        </a>
        <a class="btn btn-success" v-on:click="getRequests" title="Refresh">
          <span class="fa fa-refresh">&nbsp;</span>
        </a>
      </div>
    </div>
    <Request
      v-bind:request="request"
      v-for="request in filteredRequests"
      :key="request.correlationId"
    ></Request>
  </div>
</template>

<script>
import Request from "@/components/Request";
import { resources } from "@/resources";
import { HubConnectionBuilder } from "@aspnet/signalr";

export default {
  name: "requests",
  data() {
    return {
      filteredRequests: [],
      searchTerm: "",
      connection: {}
    };
  },
  components: {
    Request
  },
  created() {
    this.initializeSignalR();
    this.getRequests();
  },
  destroyed() {
    this.connection.stop();
  },
  computed: {
    requests() {
      return this.$store.getters.getRequests;
    }
  },
  methods: {
    search(newValue) {
      if (!newValue) {
        this.filteredRequests = this.requests;
      } else {
        this.filteredRequests = this.requests.filter(
          r =>
            (r.executingStubId && r.executingStubId.includes(newValue)) ||
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
      this.$dialog.confirm(resources.areYouSure).then(() => {
        this.$store.dispatch("clearRequests");
      });
    },
    getRequests() {
      this.$store.dispatch("getRequests");
    },
    clearInput() {
      this.searchTerm = "";
    },
    initializeSignalR() {
      this.connection = new HubConnectionBuilder()
        .withUrl("/requestHub")
        .build();
      this.connection.on("RequestReceived", request => {
        this.$store.commit("addAdditionalRequest", request);
      });
      this.connection
        .start()
        .then(() => {})
        .catch(err => {
          return console.error(err.toString());
        });
    }
  },
  watch: {
    searchTerm(newValue, oldValue) {
      this.search(newValue);
    },
    $route(from, to) {
      this.handleUrlSearch();
    },
    requests(newRequests) {
      this.filteredRequests = newRequests;
      this.handleUrlSearch();
    }
  }
};
</script>

<style scoped>
</style>