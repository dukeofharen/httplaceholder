<template>
  <div class="row">
    <div class="col-12 request" :key="request.correlationId">
        <strong class="url" v-on:click="showDetails">{{request.requestParameters.url}} (<Bool bool="request.executingStubId" trueText="executed" falseText="not executed" />)</strong>
          <div class="row" v-if="detailsVisible">
              <div class="col-2">Method</div>
              <div class="col-10">{{request.requestParameters.method}}</div>

              <div class="col-2">Body</div>
              <div class="col-10">{{request.requestParameters.body}}</div>

              <div class="col-2">Client IP</div>
              <div class="col-10">{{request.requestParameters.clientIp}}</div>

              <div class="col-2">Headers</div>
              <div class="col-10">
                  <ul>
                    <li v-for="(value, key) in request.requestParameters.headers" :key="key">{{key}}: {{value}}</li>
                  </ul>
              </div>

              <div class="col-2">Correlation ID</div>
              <div class="col-10">{{request.correlationId}}</div>

              <div class="col-2">Executed stub</div>
              <div class="col-10">{{request.executingStubId}}</div>

              <!-- TODO format with Moment.js -->
              <div class="col-2">Request begin time</div>
              <div class="col-10">{{request.requestBeginTime}}</div>

              <!-- TODO format with Moment.js -->
              <div class="col-2">Request end time</div>
              <div class="col-10">{{request.requestEndTime}}</div>
              
              <div class="col-12">
                <strong class="url" v-on:click="showExecutionResults">Stub execution results</strong>
              </div>
              <RequestStubExecutionResult v-if="executionResultsVisible" v-for="(result, key) in request.stubExecutionResults" v-bind:executionResult="result" :key="key"></RequestStubExecutionResult>

              <div class="col-12">
                  <strong class="url" v-on:click="showResponseWriterResults">Response writer results</strong>
              </div>
              <div class="col-12 row" v-if="responseWriterResultsVisible" v-for="(result, key) in request.stubResponseWriterResults" :key="key">
                <div class="col-3">Response writer name</div>
                <div class="col-9">{{result.responseWriterName}}</div>

                <div class="col-3">Executed</div>
                <div class="col-9">{{result.executed}}</div>

                <div class="col-12">
                    <hr />
                </div>
              </div>
          </div>
    </div>
  </div>
</template>

<script>
import RequestStubExecutionResult from "./RequestStubExecutionResult";
import Bool from "./Bool";

export default {
  name: "request",
  props: ["request"],
  components: {
    RequestStubExecutionResult,
    Bool
  },
  data() {
    return {
      detailsVisible: false,
      executionResultsVisible: false,
      responseWriterResultsVisible: false
    };
  },
  created() {},
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
  background-color: #f8f9fa;
  margin: 10px;
  padding: 10px;
  text-align: left;
}
.url {
  cursor: pointer;
}
</style>