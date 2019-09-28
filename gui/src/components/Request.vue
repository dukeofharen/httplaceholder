<template>
  <v-expansion-panel>
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
      <v-list-item>
        <v-btn
          @click="createStub"
          title="Create a stub based on the request parameters of this request"
          color="success"
        >Create stub</v-btn>
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
      <v-list-item v-if="showQueryParameters">
        <v-list-item-content>
          <v-list-item-title>Query parameters</v-list-item-title>
          <v-list-item-subtitle>
            <span v-for="(value, key) in queryParameters" :key="key">
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
              <v-expansion-panel v-for="(result, key) in request.stubExecutionResults" :key="key">
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
            <v-list-item v-for="(result, key) in request.stubResponseWriterResults" :key="key">
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
</template>

<script>
import RequestBody from "@/components/RequestBody";
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
    RequestBody
  },
  computed: {
    lastSelectedStub() {
      return this.$store.getters.getLastSelectedStub;
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