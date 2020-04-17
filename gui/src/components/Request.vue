<template>
  <v-expansion-panel>
    <v-expansion-panel-header>
      <span>
        <strong>{{ request.requestParameters.method }}</strong>
        {{ request.requestParameters.url }}
        <span>(</span>
        <strong>
          <Bool
            v-bind:bool="request.executingStubId"
            trueText="executed"
            falseText="not executed"
          />
        </strong>
        <span>&nbsp;|&nbsp;</span>
        <span>{{ request.requestEndTime | datetime }}</span>
        <span>)</span>
      </span>
    </v-expansion-panel-header>
    <v-expansion-panel-content>
      <v-list-item>
        <v-btn
          @click="createStub"
          title="Create a stub based on the request parameters of this request"
          color="success"
          >Create stub</v-btn
        >
      </v-list-item>
      <v-list-item v-if="request.requestParameters.body">
        <v-list-item-content>
          <v-list-item-title>Body</v-list-item-title>
          <RequestBody v-bind:requestParameters="request.requestParameters" />
        </v-list-item-content>
      </v-list-item>
      <v-list-item>
        <v-list-item-content>
          <v-list-item-title>Client IP</v-list-item-title>
          <v-list-item-subtitle>{{
            request.requestParameters.clientIp
          }}</v-list-item-subtitle>
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
          <v-list-item-subtitle>{{
            request.correlationId
          }}</v-list-item-subtitle>
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
              >{{ request.executingStubId }}</router-link
            >
          </v-list-item-subtitle>
        </v-list-item-content>
      </v-list-item>
      <v-list-item>
        <v-list-item-content>
          <v-list-item-title>Stub tenant (category)</v-list-item-title>
          <v-list-item-subtitle>
            <router-link
              :to="{ name: 'stubs', query: { stubTenant: request.stubTenant } }"
              >{{ request.stubTenant }}</router-link
            >
          </v-list-item-subtitle>
        </v-list-item-content>
      </v-list-item>
      <v-list-item>
        <v-list-item-content>
          <v-list-item-title>Request begin time</v-list-item-title>
          <v-list-item-subtitle>{{
            request.requestBeginTime | datetime
          }}</v-list-item-subtitle>
        </v-list-item-content>
      </v-list-item>
      <v-list-item>
        <v-list-item-content>
          <v-list-item-title>Request end time</v-list-item-title>
          <v-list-item-subtitle>{{
            request.requestEndTime | datetime
          }}</v-list-item-subtitle>
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
                          <v-list-item-subtitle>{{
                            condition.checkerName
                          }}</v-list-item-subtitle>
                        </v-list-item-content>
                      </v-list-item>
                      <v-list-item>
                        <v-list-item-content>
                          <v-list-item-title
                            >Condition validation</v-list-item-title
                          >
                          <v-list-item-subtitle>
                            <Bool
                              v-bind:bool="
                                condition.conditionValidation == 'Valid'
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
                          <v-list-item-subtitle>{{
                            condition.log
                          }}</v-list-item-subtitle>
                        </v-list-item-content>
                      </v-list-item>
                      <v-divider></v-divider>
                    </div>
                  </div>
                  <div v-if="result.negativeConditions.length > 0">
                    <h2>Executed negative conditions</h2>
                    <div
                      v-for="(condition, key) in result.negativeConditions"
                      :key="key"
                    >
                      <v-list-item>
                        <v-list-item-content>
                          <v-list-item-title>Checker name</v-list-item-title>
                          <v-list-item-subtitle>{{
                            condition.checkerName
                          }}</v-list-item-subtitle>
                        </v-list-item-content>
                      </v-list-item>
                      <v-list-item>
                        <v-list-item-content>
                          <v-list-item-title
                            >Condition validation</v-list-item-title
                          >
                          <v-list-item-subtitle>
                            <Bool
                              v-bind:bool="
                                condition.conditionValidation == 'Valid'
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
                          <v-list-item-subtitle>{{
                            condition.log
                          }}</v-list-item-subtitle>
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
              v-for="(result, key) in orderedStubResponseWriterResults"
              :key="key"
            >
              <v-list-item-content>
                <v-list-item-title>{{
                  result.responseWriterName
                }}</v-list-item-title>
                <v-list-item-subtitle>
                  <Bool
                    v-bind:bool="result.executed"
                    trueText="executed"
                    falseText="not executed"
                  />
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
import RequestBody from "@/components/RequestBody";
import Bool from "@/components/Bool";
import { parseUrl } from "@/functions/urlFunctions";

export default {
  name: "request",
  props: ["request"],
  data() {
    return {
      queryParameters: {},
      showQueryParameters: false
    };
  },
  created() {
    this.queryParameters = parseUrl(this.request.requestParameters.url);
    if (Object.keys(this.queryParameters).length > 0) {
      this.showQueryParameters = true;
    }
  },
  components: {
    RequestBody,
    Bool
  },
  computed: {
    lastSelectedStub() {
      return this.$store.getters.getLastSelectedStub;
    },
    orderedStubExecutionResults() {
      const compare = (a, b) => {
        if (a.passed) return -1;
        if (!a.passed) return 1;
        return 0;
      };
      return this.request.stubExecutionResults.sort(compare);
    },
    orderedStubResponseWriterResults() {
      const compare = (a, b) => {
        if (a.executed) return -1;
        if (!a.executed) return 1;
        return 0;
      };
      return this.request.stubResponseWriterResults.sort(compare);
    }
  },
  watch: {
    lastSelectedStub(fullStub) {
      this.$router.push({
        name: "updateStub",
        params: { stubId: this.lastSelectedStub.fullStub.stub.id }
      });
    }
  },
  methods: {
    createStub() {
      this.$store.dispatch("createStubBasedOnRequest", {
        correlationId: this.request.correlationId
      });
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
