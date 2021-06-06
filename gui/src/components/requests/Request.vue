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
      <v-row>
        <v-col cols="12">
          <v-btn
            @click="createStub"
            title="Create a stub based on the request parameters of this request"
            color="success"
            class="mr-2"
            >Create stub
          </v-btn>
          <v-btn
            @click="deleteRequest"
            title="Delete this request"
            color="error"
            >Delete request
          </v-btn>
        </v-col>

        <v-col cols="12">
          <span class="label">URL</span>
          <span>{{ request.requestParameters.url }}</span>
        </v-col>

        <v-col cols="12">
          <span class="label">Client IP</span>
          <span>{{ request.requestParameters.clientIp }}</span>
        </v-col>

        <v-col cols="12">
          <span class="label">Correlation ID</span>
          <span>{{ request.correlationId }}</span>
        </v-col>

        <v-col cols="12" v-if="request.executingStubId">
          <span class="label">Executed stub</span>
          <router-link
            :to="{
              name: 'stubs',
              query: { searchTerm: request.executingStubId }
            }"
            >{{ request.executingStubId }}
          </router-link>
        </v-col>

        <v-col cols="12" v-if="request.stubTenant">
          <span class="label">Stub tenant (category)</span>
          <router-link
            :to="{ name: 'stubs', query: { stubTenant: request.stubTenant } }"
            >{{ request.stubTenant }}
          </router-link>
        </v-col>

        <v-col cols="12">
          <span class="label">Request time</span>
          <span
            >{{ overviewRequest.requestEndTime | datetime }} (it took
            <em>{{ duration | decimal }}</em> ms)</span
          >
        </v-col>

        <v-col cols="12">
          <v-expansion-panels>
            <RequestHeaders :requestParameters="request.requestParameters" />
          </v-expansion-panels>
          <v-expansion-panels>
            <QueryParams
              v-if="showQueryParameters"
              :requestParameters="request.requestParameters"
            />
          </v-expansion-panels>
        </v-col>

        <v-col cols="12" v-if="request.requestParameters.body">
          <span class="label">Body</span>
          <RequestBody :requestParameters="request.requestParameters" />
        </v-col>

        <v-col cols="12">
          <v-expansion-panels>
            <StubExecutionResults
              v-if="request.stubExecutionResults.length"
              :request="request"
            />
          </v-expansion-panels>
          <v-expansion-panels>
            <ResponseWriterResults
              v-if="request.stubResponseWriterResults.length"
              :request="request"
            />
          </v-expansion-panels>
        </v-col>
      </v-row>
    </v-expansion-panel-content>
  </v-expansion-panel>
</template>

<script>
import RequestBody from "@/components/requests/RequestBody";
import Bool from "@/components/requests/Bool";
import HttpMethod from "@/components/requests/HttpMethod";
import RequestHeaders from "@/components/requests/RequestHeaders";
import QueryParams from "@/components/requests/QueryParams";
import StubExecutionResults from "@/components/requests/StubExecutionResults";
import ResponseWriterResults from "@/components/requests/ResponseWriterResults";
import { toastError, toastSuccess } from "@/utils/toastUtil";
import { resources } from "@/shared/resources";
import { routeNames } from "@/router/routerConstants";
import { setIntermediateStub } from "@/utils/sessionUtil";
import moment from "moment";
import yaml from "js-yaml";

export default {
  name: "request",
  props: ["overviewRequest"],
  data() {
    return {
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
    StubExecutionResults,
    ResponseWriterResults,
    RequestBody,
    Bool,
    HttpMethod,
    RequestHeaders,
    QueryParams
  },
  computed: {
    duration() {
      const from = new Date(this.overviewRequest.requestBeginTime);
      const to = new Date(this.overviewRequest.requestEndTime);
      return to.getTime() - from.getTime();
    },
    showQueryParameters() {
      return this.request.requestParameters.url.includes("?");
    }
  },
  methods: {
    async createStub() {
      try {
        const fullStub = await this.$store.dispatch(
          "stubs/createStubBasedOnRequest",
          {
            correlationId: this.request.correlationId,
            doNotCreateStub: true
          }
        );
        const stubYaml = yaml.dump(fullStub.stub);
        setIntermediateStub(stubYaml);
        await this.$router.push({
          name: routeNames.stubForm
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
    async deleteRequest() {
      try {
        await this.$store.dispatch("requests/deleteRequest", this.request.correlationId);
        toastSuccess(resources.requestDeletedSuccessfully);
        this.$emit("updated", this.fullStub);
      } catch(e) {
        if (e.response) {
          if (e.response.status === 404) {
            toastError(resources.requestNotFoundAnymore);
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
.label {
  font-weight: bold;
  display: block;
}
</style>
