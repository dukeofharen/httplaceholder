<template>
  <v-expansion-panel>
    <v-expansion-panel-header @click="loadRequest">
      <span>
        <strong><HttpMethod :method="overviewRequest.method" /> </strong>
        {{ overviewRequest.url }}
        <span>(</span>
        <strong>
          <Bool
            v-bind:bool="overviewRequest.executingStubId"
            trueText="executed"
            falseText="not executed"
          />
        </strong>
        <span>&nbsp;|&nbsp;</span>
        <span :title="overviewRequest.requestEndTime | datetime">{{
          timeFrom
        }}</span>
        <span>)</span>
      </span>
    </v-expansion-panel-header>
    <v-expansion-panel-content v-if="request">
      <v-list-item>
        <v-btn
          @click="createStub"
          title="Create a stub based on the request parameters of this request"
          color="success"
          >Create stub
        </v-btn>
      </v-list-item>
      <v-list-item v-if="request.requestParameters.body">
        <v-list-item-content>
          <v-list-item-title>Body</v-list-item-title>
          <RequestBody v-bind:requestParameters="request.requestParameters" />
        </v-list-item-content>
      </v-list-item>
      <v-list-item>
        <v-list-item-content>
          <v-list-item-title>URL</v-list-item-title>
          <v-list-item-subtitle
            >{{ request.requestParameters.url }}
          </v-list-item-subtitle>
        </v-list-item-content>
      </v-list-item>
      <v-list-item>
        <v-list-item-content>
          <v-list-item-title>Client IP</v-list-item-title>
          <v-list-item-subtitle
            >{{ request.requestParameters.clientIp }}
          </v-list-item-subtitle>
        </v-list-item-content>
      </v-list-item>
      <v-list-item>
        <v-list-item-content>
          <v-list-item-title>Headers</v-list-item-title>
          <v-list-item-subtitle>
            <span
              v-for="(value, key) in request.requestParameters.headers"
              :key="key"
            >
              {{ key }}: {{ value }}
              <br />
            </span>
          </v-list-item-subtitle>
        </v-list-item-content>
      </v-list-item>
      <v-list-item v-if="showQueryParameters">
        <v-list-item-content>
          <v-list-item-title>Query parameters</v-list-item-title>
          <v-list-item-subtitle>
            <span v-for="(value, key) in queryParameters" :key="key">
              {{ key }}: {{ value }}
              <br />
            </span>
          </v-list-item-subtitle>
        </v-list-item-content>
      </v-list-item>
      <v-list-item>
        <v-list-item-content>
          <v-list-item-title>Correlation ID</v-list-item-title>
          <v-list-item-subtitle
            >{{ request.correlationId }}
          </v-list-item-subtitle>
        </v-list-item-content>
      </v-list-item>
      <v-list-item>
        <v-list-item-content>
          <v-list-item-title>Executed stub</v-list-item-title>
          <v-list-item-subtitle>
            <router-link
              :to="{
                name: 'stubs',
                query: { searchTerm: request.executingStubId }
              }"
              >{{ request.executingStubId }}
            </router-link>
          </v-list-item-subtitle>
        </v-list-item-content>
      </v-list-item>
      <v-list-item>
        <v-list-item-content>
          <v-list-item-title>Stub tenant (category)</v-list-item-title>
          <v-list-item-subtitle>
            <router-link
              :to="{ name: 'stubs', query: { stubTenant: request.stubTenant } }"
              >{{ request.stubTenant }}
            </router-link>
          </v-list-item-subtitle>
        </v-list-item-content>
      </v-list-item>
      <v-list-item>
        <v-list-item-content>
          <v-list-item-title>Request time</v-list-item-title>
          <v-list-item-subtitle
            >{{ overviewRequest.requestEndTime | datetime }} (it took
            <em>{{ duration | decimal }}</em> ms)
          </v-list-item-subtitle>
        </v-list-item-content>
      </v-list-item>
      <v-expansion-panels>
        <v-expansion-panel v-if="orderedStubExecutionResults.length">
          <v-expansion-panel-header>
            <strong>Stub execution results</strong>
          </v-expansion-panel-header>
          <v-expansion-panel-content>
            <v-expansion-panels>
              <v-expansion-panel
                v-for="(result, key) in orderedStubExecutionResults"
                :key="key"
              >
                <v-expansion-panel-header>
                  <strong>
                    <span>{{ result.stubId }}</span>
                    <span>&nbsp;</span>
                    <span>(</span>
                    <span>
                      <Bool
                        v-bind:bool="result.passed"
                        trueText="passed"
                        falseText="not passed"
                      />
                    </span>
                    <span>)</span>
                  </strong>
                </v-expansion-panel-header>
                <v-expansion-panel-content>
                  <div v-if="result.conditions.length > 0">
                    <h2>Executed conditions</h2>
                    <div
                      v-for="(condition, key) in result.conditions"
                      :key="key"
                    >
                      <v-list-item>
                        <v-list-item-content>
                          <v-list-item-title>Checker name</v-list-item-title>
                          <v-list-item-subtitle
                            >{{ condition.checkerName }}
                          </v-list-item-subtitle>
                        </v-list-item-content>
                      </v-list-item>
                      <v-list-item>
                        <v-list-item-content>
                          <v-list-item-title
                            >Condition validation
                          </v-list-item-title>
                          <v-list-item-subtitle>
                            <Bool
                              v-bind:bool="
                                condition.conditionValidation ===
                                  conditionValidationType.Valid
                              "
                              trueText="passed"
                              falseText="not passed"
                            />
                          </v-list-item-subtitle>
                        </v-list-item-content>
                      </v-list-item>
                      <v-list-item v-if="condition.log">
                        <v-list-item-content>
                          <v-list-item-title>Log</v-list-item-title>
                          <v-list-item-subtitle
                            >{{ condition.log }}
                          </v-list-item-subtitle>
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
        <v-expansion-panel v-if="orderedStubResponseWriterResults.length">
          <v-expansion-panel-header>
            <strong>Response writer results</strong>
          </v-expansion-panel-header>
          <v-expansion-panel-content>
            <v-list-item
              v-for="(result, key) in orderedStubResponseWriterResults"
              :key="key"
            >
              <v-list-item-content>
                <v-list-item-title
                  >{{ result.responseWriterName }}
                </v-list-item-title>
                <v-list-item-subtitle>
                  <Bool
                    v-bind:bool="result.executed"
                    trueText="executed"
                    falseText="not executed"
                  />
                  <br />
                  <span v-if="result.log">{{ result.log }}</span>
                </v-list-item-subtitle>
              </v-list-item-content>
            </v-list-item>
          </v-expansion-panel-content>
        </v-expansion-panel>
      </v-expansion-panels>
    </v-expansion-panel-content>
  </v-expansion-panel>
</template>

<script>
import RequestBody from "@/components/requests/RequestBody";
import Bool from "@/components/requests/Bool";
import HttpMethod from "@/components/requests/HttpMethod";
import { parseUrl } from "@/utils/urlFunctions";
import { toastError, toastSuccess } from "@/utils/toastUtil";
import { resources } from "@/shared/resources";
import { routeNames } from "@/router/routerConstants";
import { conditionValidationType } from "@/shared/resources";
import moment from "moment";

export default {
  name: "request",
  props: ["overviewRequest"],
  data() {
    return {
      conditionValidationType,
      refreshTimeFromInterval: null,
      request: null,
      timeFrom: null
    };
  },
  mounted() {
    this.refreshTimeFrom();
    this.refreshTimeFromInterval = setInterval(
      () => this.refreshTimeFrom(),
      60000
    );
  },
  destroyed() {
    if (this.refreshTimeFromInterval) {
      clearInterval(this.refreshTimeFromInterval);
    }
  },
  components: {
    RequestBody,
    Bool,
    HttpMethod
  },
  computed: {
    orderedStubExecutionResults() {
      const compare = a => {
        if (a.passed) return -1;
        if (!a.passed) return 1;
        return 0;
      };
      const results = this.request.stubExecutionResults;
      results.sort(compare);
      return results;
    },
    orderedStubResponseWriterResults() {
      const compare = a => {
        if (a.executed) return -1;
        if (!a.executed) return 1;
        return 0;
      };
      const results = this.request.stubResponseWriterResults;
      results.sort(compare);
      return results;
    },
    duration() {
      const from = new Date(this.overviewRequest.requestBeginTime);
      const to = new Date(this.overviewRequest.requestEndTime);
      return to.getTime() - from.getTime();
    },
    queryParameters() {
      return parseUrl(this.request.requestParameters.url);
    },
    showQueryParameters() {
      return Object.keys(this.queryParameters).length > 0;
    }
  },
  methods: {
    async createStub() {
      try {
        const fullStub = await this.$store.dispatch(
          "stubs/createStubBasedOnRequest",
          {
            correlationId: this.request.correlationId
          }
        );
        const stub = fullStub.stub;
        toastSuccess(resources.stubAddedSuccessfully.format(stub.id));
        await this.$router.push({
          name: routeNames.stubForm,
          params: { stubId: stub.id }
        });
      } catch (e) {
        toastError(resources.stubNotAddedGeneric);
      }
    },
    async loadRequest() {
      if (!this.request) {
        try {
          this.request = await this.$store.dispatch(
            "requests/getRequest",
            this.overviewRequest.correlationId
          );
        } catch (e) {
          if (e.response) {
            if (e.response.status === 404) {
              toastError(resources.requestNotFoundAnymore);
            }
          }
        }
      }
    },
    refreshTimeFrom() {
      let date = moment(this.overviewRequest.requestEndTime);
      this.timeFrom = date.fromNow();
    }
  }
};
</script>

<style scoped>
/*noinspection CssUnusedSymbol*/
.v-chip {
  margin-right: 10px;
}
</style>
