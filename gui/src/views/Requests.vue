<template>
  <v-row>
    <v-col>
      <h1>Requests</h1>
      <v-expansion-panels>
        <v-expansion-panel
          v-for="request in filteredRequests"
          :key="request.correlationId"
          @click="requestClick(request)"
        >
          <v-expansion-panel-header>
            <span>
              <strong>{{request.requestParameters.method}}</strong>
              {{request.requestParameters.url}}
              <span>(</span>
              <strong>{{request.executingStubId ? "executed" : "not executed"}}</strong>
              <span>&nbsp;|&nbsp;</span>
              <span>{{request.requestEndTime | datetime}}</span>
              <span>)</span>
            </span>
          </v-expansion-panel-header>
          <v-expansion-panel-content>
            <v-list-item v-if="request.requestParameters.body">
              <v-list-item-content>
                <v-list-item-title>Body (TODO)</v-list-item-title>
                <v-list-item-subtitle>{{request.requestParameters.body}}</v-list-item-subtitle>
              </v-list-item-content>
            </v-list-item>
            <v-list-item>
              <v-list-item-content>
                <v-list-item-title>Client IP</v-list-item-title>
                <v-list-item-subtitle>{{request.requestParameters.clientIp}}</v-list-item-subtitle>
              </v-list-item-content>
            </v-list-item>
            <v-list-item>
              <v-list-item-content>
                <v-list-item-title>Headers</v-list-item-title>
                <v-list-item-subtitle>
                  <span v-for="(value, key) in request.requestParameters.headers" :key="key">
                    {{key}}: {{value}}
                    <br />
                  </span>
                </v-list-item-subtitle>
              </v-list-item-content>
            </v-list-item>
            <v-list-item>
              <v-list-item-content>
                <v-list-item-title>Correlation ID</v-list-item-title>
                <v-list-item-subtitle>{{request.correlationId}}</v-list-item-subtitle>
              </v-list-item-content>
            </v-list-item>
            <v-list-item>
              <v-list-item-content>
                <v-list-item-title>Executed stub (TODO)</v-list-item-title>
                <v-list-item-subtitle>{{request.executingStubId}}</v-list-item-subtitle>
              </v-list-item-content>
            </v-list-item>
            <v-list-item>
              <v-list-item-content>
                <v-list-item-title>Stub tenant (category) (TODO)</v-list-item-title>
                <v-list-item-subtitle>{{request.stubTenant}}</v-list-item-subtitle>
              </v-list-item-content>
            </v-list-item>
            <v-list-item>
              <v-list-item-content>
                <v-list-item-title>Request begin time</v-list-item-title>
                <v-list-item-subtitle>{{request.requestBeginTime | datetime}}</v-list-item-subtitle>
              </v-list-item-content>
            </v-list-item>
            <v-list-item>
              <v-list-item-content>
                <v-list-item-title>Request end time</v-list-item-title>
                <v-list-item-subtitle>{{request.requestEndTime | datetime}}</v-list-item-subtitle>
              </v-list-item-content>
            </v-list-item>
            <v-expansion-panels>
              <v-expansion-panel>
                <v-expansion-panel-header>
                  <strong>Stub execution results</strong>
                </v-expansion-panel-header>
                <v-expansion-panel-content>
                  <v-expansion-panels>
                    <v-expansion-panel
                      v-for="(result, key) in request.stubExecutionResults"
                      :key="key"
                    >
                      <v-expansion-panel-header>
                        <strong>{{result.stubId}} ({{result.passed ? "passed" : "not passed"}})</strong>
                      </v-expansion-panel-header>
                      <v-expansion-panel-content>
                        <div v-if="result.conditions.length  > 0">
                          <h2>Executed conditions</h2>
                          <div v-for="(condition, key) in result.conditions" :key="key">
                            <v-list-item>
                              <v-list-item-content>
                                <v-list-item-title>Checker name</v-list-item-title>
                                <v-list-item-subtitle>{{condition.checkerName}}</v-list-item-subtitle>
                              </v-list-item-content>
                            </v-list-item>
                            <v-list-item>
                              <v-list-item-content>
                                <v-list-item-title>Condition validation</v-list-item-title>
                                <v-list-item-subtitle>{{condition.conditionValidation}}</v-list-item-subtitle>
                              </v-list-item-content>
                            </v-list-item>
                            <v-list-item v-if="condition.log">
                              <v-list-item-content>
                                <v-list-item-title>Log</v-list-item-title>
                                <v-list-item-subtitle>{{condition.log}}</v-list-item-subtitle>
                              </v-list-item-content>
                            </v-list-item>
                            <v-divider></v-divider>
                          </div>
                        </div>
                        <div v-if="result.negativeConditions.length  > 0">
                          <h2>Executed negative conditions</h2>
                          <div v-for="(condition, key) in result.negativeConditions" :key="key">
                            <v-list-item>
                              <v-list-item-content>
                                <v-list-item-title>Checker name</v-list-item-title>
                                <v-list-item-subtitle>{{condition.checkerName}}</v-list-item-subtitle>
                              </v-list-item-content>
                            </v-list-item>
                            <v-list-item>
                              <v-list-item-content>
                                <v-list-item-title>Condition validation</v-list-item-title>
                                <v-list-item-subtitle>{{condition.conditionValidation}}</v-list-item-subtitle>
                              </v-list-item-content>
                            </v-list-item>
                            <v-list-item v-if="condition.log">
                              <v-list-item-content>
                                <v-list-item-title>Log</v-list-item-title>
                                <v-list-item-subtitle>{{condition.log}}</v-list-item-subtitle>
                              </v-list-item-content>
                            </v-list-item>
                            <v-divider></v-divider>
                          </div>
                        </div>
                      </v-expansion-panel-content>
                    </v-expansion-panel>
                  </v-expansion-panels>
                </v-expansion-panel-content>
              </v-expansion-panel>
              <v-expansion-panel>
                <v-expansion-panel-header>
                  <strong>Response writer results</strong>
                </v-expansion-panel-header>
                <v-expansion-panel-content>
                  <v-list-item
                    v-for="(result, key) in request.stubResponseWriterResults"
                    :key="key"
                  >
                    <v-list-item-content>
                      <v-list-item-title>{{result.responseWriterName}}</v-list-item-title>
                      <v-list-item-subtitle>{{result.executed ? "executed" : "not executed"}}</v-list-item-subtitle>
                    </v-list-item-content>
                  </v-list-item>
                </v-expansion-panel-content>
              </v-expansion-panel>
            </v-expansion-panels>
          </v-expansion-panel-content>
        </v-expansion-panel>
      </v-expansion-panels>
    </v-col>
  </v-row>
  <!-- <h1>Requests</h1>
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
    },
    requestClick(request) {}
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
.v-chip {
  margin-right: 10px;
}

.request {
  margin-bottom: 20px;
}
</style>