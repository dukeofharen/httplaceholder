<template>
  <v-row v-shortkey="['ctrl', 's']" @shortkey="saveStub">
    <v-col>
      <h1>Add stub</h1>
      <v-card>
        <v-card-title>You can add a new stub here</v-card-title>
        <v-card-text>
          <v-row>
            <v-col>
              Use the form below to specify the request conditions and which response should be returned when the
              request conditions all match.
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
      <v-card>
        <v-card-title>General</v-card-title>
        <v-card-text>
          <v-row>
            <v-col>
              <!-- ID -->
              <div class="d-flex flex-row mb-6">
                <FormTooltip tooltipKey="id"/>
                <v-text-field v-model="stubId" :label="formLabels.id" class="pa-2"/>
              </div>

              <!-- Tenant -->
              <div class="d-flex flex-row mb-6">
                <FormTooltip tooltipKey="tenant"/>
                <v-menu absolute offset-y>
                  <template v-slot:activator="{on}">
                    <v-text-field v-model="stubTenant" :label="formLabels.tenant" v-on="on" clearable
                                  class="pa-2"/>
                  </template>
                  <v-list>
                    <v-list-item v-for="(tenant, index) in filteredTenantNames" :key="index"
                                 @click="stubTenant = tenant">
                      <v-list-item-title>{{tenant}}</v-list-item-title>
                    </v-list-item>
                  </v-list>
                </v-menu>
              </div>

              <!-- Description -->
              <div class="d-flex flex-row mb-6">
                <FormTooltip tooltipKey="description"/>
                <v-textarea v-model="stubDescription" :label="formLabels.description"/>
              </div>

              <!-- Priority -->
              <div class="d-flex flex-row mb-6">
                <FormTooltip tooltipKey="priority"/>
                <v-text-field v-model="stubPriority" :label="formLabels.priority" class="pa-2"/>
              </div>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
      <v-card>
        <v-card-title>Request conditions</v-card-title>
        <v-card-text>
          <v-row>
            <v-col>
              <div>
                <h2 class="section-title" @click="show.generalConditions = !show.generalConditions">
                  <v-icon>{{show.generalConditions ? "mdi-chevron-down" : "mdi-chevron-right"}}</v-icon>
                  General
                  conditions
                </h2>
              </div>

              <div v-if="show.generalConditions">
                <!-- HTTP method -->
                <div class="d-flex flex-row mb-6">
                  <FormTooltip tooltipKey="httpMethod"/>
                  <v-menu absolute offset-y>
                    <template v-slot:activator="{on}">
                      <v-text-field v-model="stubConditionMethod" :label="formLabels.httpMethod" v-on="on"
                                    clearable
                                    class="pa-2"/>
                    </template>
                    <v-list>
                      <v-list-item v-for="(method, index) in httpMethods" :key="index"
                                   @click="stubConditionMethod = method">
                        <v-list-item-title>{{method}}</v-list-item-title>
                      </v-list-item>
                    </v-list>
                  </v-menu>
                </div>

                <!-- Client IP -->
                <div class="d-flex flex-row mb-6">
                  <FormTooltip tooltipKey="clientIp"/>
                  <v-text-field v-model="stubConditionClientIp" :label="formLabels.clientIp" class="pa-2"
                                :placeholder="formPlaceholderResources.clientIp"/>
                </div>

                <!-- Hostname -->
                <div class="d-flex flex-row mb-6">
                  <FormTooltip tooltipKey="hostname"/>
                  <v-text-field v-model="stubConditionHostname" :label="formLabels.hostname" class="pa-2"
                                :placeholder="formPlaceholderResources.hostname"/>
                </div>
              </div>

              <div>
                <h2 class="section-title" @click="show.urlConditions = !show.urlConditions">
                  <v-icon>{{show.urlConditions ? "mdi-chevron-down" : "mdi-chevron-right"}}</v-icon>
                  URL conditions
                </h2>
              </div>

              <div v-if="show.urlConditions">
                <!-- URL path -->
                <div class="d-flex flex-row mb-6">
                  <FormTooltip tooltipKey="urlPath"/>
                  <v-text-field v-model="stubConditionUrlPath" :label="formLabels.urlPath" class="pa-2"
                                :placeholder="formPlaceholderResources.urlPath"/>
                </div>

                <!-- Query string -->
                <div class="d-flex flex-row mb-6">
                  <FormTooltip tooltipKey="queryString"/>
                  <v-textarea v-model="stubQueryStrings" :label="formLabels.queryString"
                              :placeholder="formPlaceholderResources.queryString"/>
                </div>

                <!-- Full path -->
                <div class="d-flex flex-row mb-6">
                  <FormTooltip tooltipKey="fullPath"/>
                  <v-text-field v-model="stubConditionUrlFullPath" :label="formLabels.fullPath" class="pa-2"
                                :placeholder="formPlaceholderResources.fullPath"/>
                </div>

                <!-- Is HTTPS -->
                <div class="d-flex flex-row mb-6">
                  <FormTooltip tooltipKey="isHttps"/>
                  <v-radio-group v-model="stubIsHttp" class="pa-2">
                    <v-radio :value="isHttpsValues.onlyHttps" :label="formLabels.onlyHttps"></v-radio>
                    <v-radio :value="isHttpsValues.onlyHttp" :label="formLabels.onlyHttp"></v-radio>
                    <v-radio :value="isHttpsValues.httpAndHttps" :label="formLabels.httpAndHttps"></v-radio>
                  </v-radio-group>
                </div>
              </div>

              <div>
                <h2 class="section-title" @click="show.headerConditions = !show.headerConditions">
                  <v-icon>{{show.headerConditions ? "mdi-chevron-down" : "mdi-chevron-right"}}</v-icon>
                  Header conditions
                </h2>
              </div>

              <div v-if="show.headerConditions">
                <!-- Headers -->
                <div class="d-flex flex-row mb-6">
                  <FormTooltip tooltipKey="headers"/>
                  <v-textarea v-model="stubHeaders" :label="formLabels.headers"
                              :placeholder="formPlaceholderResources.headers"/>
                </div>
              </div>

              <div v-if="showBodyConditionForms">
                <h2 class="section-title" @click="show.bodyConditions = !show.bodyConditions">
                  <v-icon>{{show.bodyConditions ? "mdi-chevron-down" : "mdi-chevron-right"}}</v-icon>
                  Body conditions
                </h2>
              </div>

              <div v-if="show.bodyConditions">
                <!-- Body -->
                <div class="d-flex flex-row mb-6" v-if="showBodyConditionForms">
                  <FormTooltip tooltipKey="body"/>
                  <v-textarea v-model="stubBody" :label="formLabels.body"
                              :placeholder="formPlaceholderResources.body"/>
                </div>
              </div>

              <div v-if="showBodyConditionForms">
                <h2 class="section-title" @click="show.formBodyConditions = !show.formBodyConditions">
                  <v-icon>{{show.formBodyConditions ? "mdi-chevron-down" : "mdi-chevron-right"}}</v-icon>
                  Form body conditions
                </h2>
              </div>

              <div v-if="show.formBodyConditions">
                <!-- Form body -->
                <div class="d-flex flex-row mb-6" v-if="showBodyConditionForms">
                  <FormTooltip tooltipKey="formBody"/>
                  <v-textarea v-model="stubFormBody" :label="formLabels.formBody"
                              :placeholder="formPlaceholderResources.formBody"/>
                </div>
              </div>

              <div v-if="showBodyConditionForms">
                <h2 class="section-title" @click="show.xmlBodyConditions = !show.xmlBodyConditions">
                  <v-icon>{{show.xmlBodyConditions ? "mdi-chevron-down" : "mdi-chevron-right"}}</v-icon>
                  XML body conditions
                </h2>
              </div>

              <div v-if="show.xmlBodyConditions">
                <!-- XPath -->
                <div class="d-flex flex-row mb-6" v-if="showBodyConditionForms">
                  <FormTooltip tooltipKey="xpath"/>
                  <v-textarea v-model="stubXPath" :label="formLabels.xpath"
                              :placeholder="formPlaceholderResources.xpath"/>
                </div>

                <div class="d-flex flex-row mb-6" v-if="showBodyConditionForms">
                  <FormTooltip tooltipKey="xpathNamespaces"/>
                  <v-textarea v-model="stubXPathNamespaces" :label="formLabels.xpathNamespaces"
                              :placeholder="formPlaceholderResources.xpathNamespaces"/>
                </div>
              </div>

              <div v-if="showBodyConditionForms">
                <h2 class="section-title" @click="show.jsonBodyConditions = !show.jsonBodyConditions">
                  <v-icon>{{show.jsonBodyConditions ? "mdi-chevron-down" : "mdi-chevron-right"}}</v-icon>
                  JSON body conditions
                </h2>
              </div>

              <div v-if="show.jsonBodyConditions">
                <!-- JSONPath -->
                <div class="d-flex flex-row mb-6" v-if="showBodyConditionForms">
                  <FormTooltip tooltipKey="jsonPath"/>
                  <v-textarea v-model="stubJsonPath" :label="formLabels.jsonPath"
                              :placeholder="formPlaceholderResources.jsonPath"/>
                </div>
              </div>

              <div>
                <h2 class="section-title" @click="show.authenticationConditions = !show.authenticationConditions">
                  <v-icon>{{show.authenticationConditions ? "mdi-chevron-down" : "mdi-chevron-right"}}</v-icon>
                  Authentication conditions
                </h2>
              </div>

              <div v-if="show.authenticationConditions">
                <!-- Basic authentication -->
                <div class="d-flex flex-row mb-6">
                  <FormTooltip tooltipKey="fullPath"/>
                  <v-text-field v-model="stubConditionBasicAuthUsername"
                                :label="formLabels.basicAuthUsername" class="pa-2"/>
                  <v-text-field v-model="stubConditionBasicAuthPassword"
                                :label="formLabels.basicAuthPassword" class="pa-2"/>
                </div>
              </div>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>

      <v-card>
        <v-card-title>Response writers</v-card-title>
        <v-card-text>
          <v-row>
            <v-col>

              <div>
                <h2 class="section-title" @click="show.generalWriters = !show.generalWriters">
                  <v-icon>{{show.generalWriters ? "mdi-chevron-down" : "mdi-chevron-right"}}</v-icon>
                  General writers
                </h2>
              </div>

              <div v-if="show.generalWriters">
                <!-- Status code -->
                <div class="d-flex flex-row mb-6">
                  <FormTooltip tooltipKey="statusCode"/>
                  <v-menu absolute offset-y>
                    <template v-slot:activator="{on}">
                      <v-text-field v-model="stubResponseStatusCode" :label="formLabels.statusCode" v-on="on"
                                    clearable
                                    class="pa-2"/>
                    </template>
                    <v-list max-height="300px">
                      <v-list-item v-for="code in formattedStatusCodes" :key="code.code"
                                   @click="stubResponseStatusCode = code.code">
                        <v-list-item-title>{{code.text}}</v-list-item-title>
                      </v-list-item>
                    </v-list>
                  </v-menu>
                </div>

                <!-- Extra duration -->
                <div class="d-flex flex-row mb-6">
                  <FormTooltip tooltipKey="extraDuration"/>
                  <v-text-field v-model="stubResponseExtraDuration" :label="formLabels.extraDuration" class="pa-2"/>
                </div>

                <!-- Dynamic mode -->
                <div class="d-flex flex-row mb-6">
                  <FormTooltip tooltipKey="dynamicMode"/>
                  <v-switch v-model="stubResponseDynamicMode" :label="formLabels.enableDynamicMode"
                            class="pa-2"/>
                </div>
              </div>

              <div>
                <h2 class="section-title" @click="show.bodyWriters = !show.bodyWriters">
                  <v-icon>{{show.bodyWriters ? "mdi-chevron-down" : "mdi-chevron-right"}}</v-icon>
                  Body writers
                </h2>
              </div>

              <div v-if="show.bodyWriters">
                <!-- Response body type -->
                <div class="d-flex flex-row mb-6">
                  <FormTooltip tooltipKey="responseBodyType"/>
                  <v-select v-model="stubBodyResponseType" :items="responseBodyTypes" item-text="value"
                            item-value="value"
                            :label="formLabels.responseBodyType"/>
                </div>

                <!-- Response body -->
                <div class="d-flex flex-row mb-6" v-if="showResponseBodyForm">
                  <FormTooltip tooltipKey="responseBody"/>
                  <v-textarea v-model="stubResponseBody" :label="formLabels.responseBody"
                              :placeholder="formPlaceholderResources.responseBody"
                              id="response-body"/>
                </div>

                <!-- Body variable handler -->
                <div class="d-flex flex-row mb-6" v-if="showResponseBodyForm && dynamicModeEnabled">
                  <FormTooltip tooltipKey="selectVariableHandler"/>
                  <v-menu absolute offset-y>
                    <template v-slot:activator="{on}">
                      <v-btn color="success" v-on="on">Select variable handler</v-btn>
                    </template>
                    <v-list max-height="300px">
                      <v-list-item v-for="handler in variableHandlers" :key="handler.name"
                                   @click="insertHandlerInBody(handler)">
                        <v-list-item-title>{{handler.name}}: {{handler.fullName}}. Example: {{handler.example}}
                        </v-list-item-title>
                      </v-list-item>
                    </v-list>
                  </v-menu>
                </div>
              </div>

              <div>
                <h2 class="section-title" @click="show.headerWriters = !show.headerWriters">
                  <v-icon>{{show.headerWriters ? "mdi-chevron-down" : "mdi-chevron-right"}}</v-icon>
                  Header writers
                </h2>
              </div>

              <div v-if="show.headerWriters">
                <!-- Headers -->
                <div class="d-flex flex-row mb-6">
                  <FormTooltip tooltipKey="responseHeaders"/>
                  <v-textarea v-model="stubResponseHeaders" :label="formLabels.responseHeaders"
                              :placeholder="formPlaceholderResources.responseHeaders"
                              id="response-headers"/>
                </div>

                <!-- Headers variable handler -->
                <div class="d-flex flex-row mb-6" v-if="dynamicModeEnabled">
                  <FormTooltip tooltipKey="selectVariableHandler"/>
                  <v-menu absolute offset-y>
                    <template v-slot:activator="{on}">
                      <v-btn color="success" v-on="on">Select variable handler</v-btn>
                    </template>
                    <v-list max-height="300px">
                      <v-list-item v-for="handler in variableHandlers" :key="handler.name"
                                   @click="insertHandlerInHeaders(handler)">
                        <v-list-item-title>{{handler.name}}: {{handler.fullName}}. Example: {{handler.example}}
                        </v-list-item-title>
                      </v-list-item>
                    </v-list>
                  </v-menu>
                </div>
              </div>

              <div>
                <h2 class="section-title" @click="show.redirectWriters = !show.redirectWriters">
                  <v-icon>{{show.redirectWriters ? "mdi-chevron-down" : "mdi-chevron-right"}}</v-icon>
                  Redirection writers
                </h2>
              </div>

              <div v-if="show.redirectWriters">
                <div class="d-flex flex-row mb-6">
                  <FormTooltip tooltipKey="redirect"/>
                  <v-text-field v-model="stubResponseTempRedirect" :label="formLabels.temporaryRedirect"
                                class="pa-2"
                                :placeholder="formPlaceholderResources.redirect"/>
                </div>

                <div class="d-flex flex-row mb-6">
                  <FormTooltip tooltipKey="redirect"/>
                  <v-text-field v-model="stubResponsePermanentRedirect" :label="formLabels.permanentRedirect"
                                class="pa-2"
                                :placeholder="formPlaceholderResources.redirect"/>
                </div>
              </div>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
      <v-btn color="success" @click="saveStub">Save stub</v-btn>
    </v-col>
  </v-row>
</template>

<script>
  import {actionNames} from "@/store/storeConstants";
  import FormTooltip from "@/components/FormTooltip";
  import {toastError, toastSuccess} from "@/utils/toastUtil";
  import {resources} from "@/shared/resources";
  import {
    formPlaceholderResources,
    formValidationMessages,
    formLabels,
    isHttpsValues,
    httpMethods,
    httpStatusCodes,
    responseBodyTypes
  } from "@/shared/stubFormResources";
  import {mapFields} from "vuex-map-fields";

  export default {
    name: "stubForm",
    components: {FormTooltip},
    async mounted() {
      this.tenantNames = await this.$store.dispatch(
        actionNames.getTenantNames
      );
      await this.initialize();
    },
    data() {
      return {
        show: {
          generalConditions: true,
          urlConditions: false,
          headerConditions: false,
          bodyConditions: false,
          formBodyConditions: false,
          xmlBodyConditions: false,
          jsonBodyConditions: false,
          authenticationConditions: false,
          generalWriters: true,
          bodyWriters: false,
          headerWriters: false,
          redirectWriters: false
        },
        tenantNames: [],
        httpMethods,
        formPlaceholderResources,
        formLabels,
        isHttpsValues
      };
    },
    computed: {
      filteredTenantNames() {
        if (!this.stubTenant) {
          return this.tenantNames;
        }

        return this.tenantNames.filter(t => t.includes(this.stubTenant));
      },
      formattedStatusCodes() {
        return httpStatusCodes.map(c => ({code: c.code, text: `${c.code} - ${c.name}`}));
      },
      responseBodyTypes() {
        const keys = Object.keys(responseBodyTypes);
        return keys.map(k => ({key: k, value: responseBodyTypes[k]}));
      },
      showResponseBodyForm() {
        return this.stubBodyResponseType !== responseBodyTypes.empty;
      },
      showBodyConditionForms() {
        return this.stubConditionMethod !== "GET";
      },
      dynamicModeEnabled() {
        return this.variableHandlers.length;
      },
      variableHandlers() {
        return this.$store.getters.getVariableHandlers;
      },
      ...mapFields({
        stub: "stubForm.stub",
        stubId: "stubForm.stub.id",
        stubTenant: "stubForm.stub.tenant",
        stubDescription: "stubForm.stub.description",
        stubPriority: "stubForm.stub.priority",
        stubConditionMethod: "stubForm.stub.conditions.method",
        stubConditionClientIp: "stubForm.stub.conditions.clientIp",
        stubConditionHostname: "stubForm.stub.conditions.hostname",
        stubConditionUrlPath: "stubForm.stub.conditions.url.path",
        stubQueryStrings: "stubForm.queryStrings",
        stubConditionUrlFullPath: "stubForm.stub.conditions.url.fullPath",
        stubIsHttp: "stubForm.isHttps",
        stubHeaders: "stubForm.headers",
        stubBody: "stubForm.body",
        stubFormBody: "stubForm.formBody",
        stubXPath: "stubForm.xpath",
        stubXPathNamespaces: "stubForm.xpathNamespaces",
        stubJsonPath: "stubForm.jsonPath",
        stubConditionBasicAuthUsername: "stubForm.stub.conditions.basicAuthentication.username",
        stubConditionBasicAuthPassword: "stubForm.stub.conditions.basicAuthentication.password",
        stubResponseStatusCode: "stubForm.stub.response.statusCode",
        stubResponseExtraDuration: "stubForm.stub.response.extraDuration",
        stubResponseDynamicMode: "stubForm.stub.response.enableDynamicMode",
        stubBodyResponseType: "stubForm.bodyResponseType",
        stubResponseBody: "stubForm.responseBody",
        stubResponseHeaders: "stubForm.responseHeaders",
        stubResponseTempRedirect: "stubForm.stub.response.temporaryRedirect",
        stubResponsePermanentRedirect: "stubForm.stub.response.permanentRedirect"
      })
    },
    methods: {
      async initialize() {
        const id = this.$route.params.id;
        if (id) {
          const fullStub = await this.$store.dispatch(actionNames.getStub, {
            stubId: id
          });
          this.stub = fullStub.stub;
        }
      },
      async saveStub() {
        const messages = this.$store.getters.getStubFormValidation;
        if (messages.length) {
          for (let message of messages) {
            toastError(message);
          }

          return;
        }

        const id = this.$route.params.id;
        if (id) {
          try {
            await this.$store.dispatch(actionNames.updateStub, {
              input: this.$store.getters.getStubForSaving,
              inputIsJson: true,
              id
            });
            toastSuccess(resources.stubUpdatedSuccessfully.format(id));
          } catch (e) {
            if (e.response) {
              if (e.response.status === 409) {
                toastError(resources.stubAlreadyAdded.format(id));
              } else {
                toastError(resources.stubNotAdded.format(id));
              }
            } else {
              toastError(e);
            }
          }
        } else {
          try {
            const results = await this.$store.dispatch(actionNames.addStubs, {
              input: this.$store.getters.getStubForSaving,
              inputIsJson: true
            });
            for (let result of results) {
              if (result.v) {
                toastSuccess(resources.stubAddedSuccessfully.format(result.v.stub.id));
              } else if (result.e) {
                toastError(resources.stubNotAdded.format(result.e.stubId));
              }
            }
          } catch (e) {
            toastError(e);
          }
        }

      },
      insertHandlerInBody(handler) {
        this.stubResponseBody = this.insertHandler(handler, this.stubResponseBody || "", "response-body");
      },
      insertHandlerInHeaders(handler) {
        this.stubResponseHeaders = this.insertHandler(handler, this.stubResponseHeaders || "", "response-headers");
      },
      insertHandler(handler, text, elementId) {
        this.stubResponseDynamicMode = true;
        const elem = document.getElementById(elementId);
        const position = elem.selectionStart || 0;
        return [text.slice(0, position), handler.example, text.slice(position)].join("");
      }
    },
    watch: {
      '$route.params.id': {
        async handler() {
          await this.initialize();
        }
      },
      stub: {
        deep: true,
        handler() {
          if (this.stub.id === "") {
            this.stub.id = null;
          }

          if (this.stub.tenant === "") {
            this.stub.tenant = null;
          }

          if (this.stub.description === "") {
            this.stub.description = null;
          }

          if (this.stub.conditions.clientIp === "") {
            this.stub.conditions.clientIp = null;
          }

          if (this.stub.conditions.hostname === "") {
            this.stub.conditions.hostname = null;
          }

          if (this.stub.conditions.method === "") {
            this.stub.conditions.method = null;
          }

          if (this.stub.conditions.url.path === "") {
            this.stub.conditions.url.path = null;
          }

          if (this.stub.conditions.url.fullPath === "") {
            this.stub.conditions.url.fullPath = null;
          }

          if (this.stub.conditions.basicAuthentication.username === "") {
            this.stub.conditions.basicAuthentication.username = null;
          }

          if (this.stub.conditions.basicAuthentication.password === "") {
            this.stub.conditions.basicAuthentication.password = null;
          }

          if (this.stub.response.statusCode === "") {
            this.stub.response.statusCode = null;
          }

          if (this.stub.response.extraDuration === "") {
            this.stub.response.extraDuration = null;
          }

          if (this.stub.response.temporaryRedirect === "") {
            this.stub.response.temporaryRedirect = null;
          }

          if (this.stub.response.permanentRedirect === "") {
            this.stub.response.permanentRedirect = null;
          }
        }
      }
    }
  };
</script>

<style scoped>
  .v-card {
    margin-top: 10px;
    margin-bottom: 10px;
  }

  .v-list-item {
    background-color: #ffffff !important;
  }

  h2.section-title {
    color: #112b00;
    cursor: pointer;
    margin-bottom: 10px;
  }
</style>
