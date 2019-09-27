<template>
  <v-row>
    <v-col>
      <h1>Requests</h1>
      <v-row>
        <v-col cols="6">
          <v-text-field
            v-model="searchTerm"
            placeholder="Filter on stub ID or URL..."
            clearable
          ></v-text-field>
          <v-select
            :items="tenantNames"
            placeholder="Select stub tenant / category name for the stubs you would like to see the requests for..."
            v-model="selectedTenantName"
            clearable
          ></v-select>
        </v-col>
      </v-row>
      <v-expansion-panels>
        <Request v-for="request in filteredRequests" :key="request.correlationId" v-bind:request="request"></Request>
      </v-expansion-panels>
    </v-col>
  </v-row>
  <!-- <h1>Requests</h1>
    <div class="row">
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
  ></Request>-->
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
      selectedTenantName: "",
      connection: {}
    };
  },
  components: {
    Request
  },
  created() {
    this.initializeSignalR();
    this.getRequests();
    this.getTenantNames();
  },
  destroyed() {
    this.connection.stop();
  },
  computed: {
    requests() {
      return this.$store.getters.getRequests;
    },
    tenantNames() {
      return this.$store.getters.getTenantNames;
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
    getTenantNames() {
      this.$store.dispatch("getTenantNames");
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
    },
    selectedTenantName(val) {
      if (!val) {
        this.filteredRequests = this.requests;
      } else {
        this.filteredRequests = this.requests.filter(
          req => req.stubTenant === val
        );
      }
    }
  }
};
</script>

<style scoped>
</style>