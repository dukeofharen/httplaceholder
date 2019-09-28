<template>
  <v-row>
    <v-col>
      <h1>Requests</h1>
      <v-row>
        <v-col>
          <v-text-field v-model="searchTerm" placeholder="Filter on stub ID or URL..." clearable></v-text-field>
          <v-select
            :items="tenantNames"
            placeholder="Select stub tenant / category name for the stubs you would like to see the requests for..."
            v-model="selectedTenantName"
            clearable
          ></v-select>
        </v-col>
      </v-row>
      <v-row>
        <v-col class="buttons">
          <v-btn
            title="Delete all requests"
            @click.stop="deleteAllDialog = true"
            color="error"
          >Delete all requests</v-btn>
          <v-btn title="Refresh" @click="getRequests" color="success">Refresh</v-btn>
        </v-col>
      </v-row>
      <v-expansion-panels>
        <Request
          v-for="request in filteredRequests"
          :key="request.correlationId"
          v-bind:request="request"
        ></Request>
      </v-expansion-panels>
      <v-dialog v-model="deleteAllDialog" max-width="290">
        <v-card>
          <v-card-title class="headline">Delete all requests?</v-card-title>
          <v-card-text>The requests can't be recovered.</v-card-text>
          <v-card-actions>
            <div class="flex-grow-1"></div>
            <v-btn color="green darken-1" text @click="deleteAllDialog = false">No</v-btn>
            <v-btn color="green darken-1" text @click="deleteAllRequests">Yes</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-col>
  </v-row>
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
      connection: {},
      deleteAllDialog: false
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
      this.deleteAllDialog = false;
      this.$store.dispatch("clearRequests");
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
.buttons > button {
  margin-right: 10px;
  margin-top: 10px;
}
</style>