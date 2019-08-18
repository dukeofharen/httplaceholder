<template>
  <div class="col-12 request" :key="request.correlationId">
    <strong class="url" v-on:click="showDetails">
      <HttpMethod v-bind:method="request.requestParameters.method"/>
      {{request.requestParameters.url}}
      <span>(</span>
      <Bool v-bind:bool="request.executingStubId" trueText="executed" falseText="not executed"/>
      <span>)</span>
    </strong>
    <div class="row" v-if="detailsVisible">
      <div class="col-2">Method</div>
      <div class="col-10">{{request.requestParameters.method}}</div>

      <div class="col-2">Body</div>
      <div class="col-10">
        <RequestBody v-bind:requestParameters="request.requestParameters"/>
      </div>

      <div class="col-2">Client IP</div>
      <div class="col-10">{{request.requestParameters.clientIp}}</div>

      <div class="col-2">Headers</div>
      <div class="col-10">
        <ul>
          <li
            v-for="(value, key) in request.requestParameters.headers"
            :key="key"
          >{{key}}: {{value}}</li>
        </ul>
      </div>

        <div class="col-2" v-if="showQueryParameters">Query parameters</div>
        <div class="col-10" v-if="showQueryParameters">
          <ul>
            <li v-for="(value, key) in queryParameters" :key="key">{{key}}: {{value}}</li>
          </ul>
        </div>

      <div class="col-2">Correlation ID</div>
      <div class="col-10">{{request.correlationId}}</div>

      <div class="col-2">Executed stub</div>
      <div class="col-10">
        <router-link
          :to="{ name: 'stubs', query: { searchTerm: request.executingStubId }}"
        >{{request.executingStubId}}</router-link>
      </div>

      <div class="col-2">Request begin time</div>
      <div class="col-10">{{request.requestBeginTime | datetime}}</div>

      <div class="col-2">Request end time</div>
      <div class="col-10">{{request.requestEndTime | datetime}}</div>

      <div class="col-12">
        <strong class="url" v-on:click="showExecutionResults">Stub execution results</strong>
      </div>
      <div v-if="executionResultsVisible">
        <RequestStubExecutionResult
          v-for="(result, key) in request.stubExecutionResults"
          v-bind:executionResult="result"
          :key="key"
        ></RequestStubExecutionResult>
      </div>

      <div class="col-12">
        <strong class="url" v-on:click="showResponseWriterResults">Response writer results</strong>
      </div>
      <div v-if="responseWriterResultsVisible">
        <div
          class="col-12 row"
          v-for="(result, key) in request.stubResponseWriterResults"
          :key="key"
        >
          <div class="col-3">Response writer name</div>
          <div class="col-9">{{result.responseWriterName}}</div>

          <div class="col-3">Executed</div>
          <div class="col-9">
            <Bool v-bind:bool="result.executed"/>
          </div>

          <div class="col-12">
            <hr>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import RequestStubExecutionResult from "./RequestStubExecutionResult";
import Bool from "./Bool";
import HttpMethod from "./HttpMethod";
import RequestBody from "./RequestBody";
import { parseUrl } from "@/functions/urlFunctions";

export default {
  name: "request",
  props: ["request"],
  components: {
    RequestStubExecutionResult,
    Bool,
    HttpMethod,
    RequestBody
  },
  data() {
    return {
      detailsVisible: false,
      executionResultsVisible: false,
      responseWriterResultsVisible: false,
      queryParameters: {},
      showQueryParameters: false
    };
  },
  created() {
    this.queryParameters = parseUrl(this.request.requestParameters.url);
    if(Object.keys(this.queryParameters).length > 0){
      this.showQueryParameters = true;
    }
  },
  methods: {
    showDetails() {
      this.detailsVisible = !this.detailsVisible;
    },
    showExecutionResults() {
      this.executionResultsVisible = !this.executionResultsVisible;
    },
    showResponseWriterResults() {
      this.responseWriterResultsVisible = !this.responseWriterResultsVisible;
    }
  }
};
</script>

<style scoped>
.request {
  padding: 10px;
  text-align: left;
}
.url {
  cursor: pointer;
}
</style>