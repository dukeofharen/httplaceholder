<template>
  <v-row>
    <v-col>
      <h1>Requests</h1>
      <v-row>
        <v-col class="buttons">
          <v-btn title="Refresh" @click="initialize" color="success"
          >Refresh
          </v-btn>
          <v-btn
            title="Delete all requests"
            @click.stop="deleteAllDialog = true"
            color="error"
          >Delete all requests
          </v-btn>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
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
        <Request
          v-for="request in filteredRequests"
          :key="request.correlationId"
          :overviewRequest="request"
        ></Request>
      </v-expansion-panels>
      <v-dialog v-model="deleteAllDialog" max-width="290">
        <v-card>
          <v-card-title class="headline">Delete all requests?</v-card-title>
          <v-card-text>The requests can't be recovered.</v-card-text>
          <v-card-actions>
            <div class="flex-grow-1"></div>
            <v-btn color="green darken-1" text @click="deleteAllDialog = false"
            >No
            </v-btn>
            <v-btn color="green darken-1" text @click="deleteAllRequests"
            >Yes
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-col>
  </v-row>
</template>

<script>
  import Request from "@/components/Request";
  import {HubConnectionBuilder} from "@aspnet/signalr";
  import {actionNames} from "@/store/storeConstants";
  import {toastSuccess} from "@/utils/toastUtil";
  import {resources} from "@/shared/resources";

  export default {
    name: "requests",
    data() {
      return {
        requests: [],
        tenantNames: [],
        searchTerm: "",
        selectedTenantName: "",
        connection: {},
        deleteAllDialog: false
      };
    },
    components: {
      Request
    },
    async created() {
      this.initializeSignalR();
      await this.initialize();
    },
    destroyed() {
      this.connection.stop();
    },
    computed: {
      filteredRequests() {
        let result = this.requests;
        if (this.searchTerm) {
          result = result.filter(
            r => {
              const searchTerm = this.searchTerm.toLowerCase();
              const stubId = r.executingStubId ? r.executingStubId.toLowerCase() : "";
              const url = r.url.toLowerCase();
              return stubId &&
                stubId.includes(searchTerm) ||
                url.includes(searchTerm);
            }
          );
        }

        if (this.selectedTenantName) {
          result = result.filter(r => r.stubTenant === this.selectedTenantName);
        }

        return result;
      }
    },
    methods: {
      async initialize() {
        const getRequestsPromise = this.$store.dispatch(actionNames.getRequestsOverview);
        const getTenantNamesPromise = this.$store.dispatch(
          actionNames.getTenantNames
        );
        this.requests = await getRequestsPromise;
        this.tenantNames = await getTenantNamesPromise;

        this.initializeSearch();
      },
      initializeSearch() {
        this.searchTerm = this.$route.query.searchTerm;
        this.selectedTenantName = this.$route.query.stubTenant;
      },
      async deleteAllRequests() {
        this.deleteAllDialog = false;
        await this.$store.dispatch(actionNames.clearRequests);
        toastSuccess(resources.requestsDeletedSuccessfully);
        this.requests = [];
      },
      initializeSignalR() {
        this.connection = new HubConnectionBuilder()
          .withUrl("/requestHub")
          .build();
        this.connection.on("RequestReceived", request =>
          this.requests.unshift(request)
        );
        this.connection
          .start()
          .then(() => {
          })
          .catch(err => console.error(err.toString()));
      }
    },
    watch: {
      $route() {
        this.initializeSearch();
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
