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
          />
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
    <div class="row">
      <div class="input-group col-md-12" v-if="tenantNames.length > 0">
        <select v-model="selectedTenantName" class="form-control tenant-list">
          <option
            selected="selected"
            value=""
          >Select stub tenant / category name for the stubs you would like to see the requests for...</option>
          <option
            v-for="tenantName in tenantNames"
            v-bind:key="tenantName"
            v-bind:value="tenantName"
          >{{tenantName}}</option>
        </select>
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
.tenant-list {
  margin-top: 10px;
}
</style>