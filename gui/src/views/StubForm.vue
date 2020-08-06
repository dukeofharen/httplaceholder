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
                              :placeholder="formPlaceholderResources.body" @keyup="bodyChanged"/>
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

              <!--              <div v-if="showBodyConditionForms">-->
              <!--                <h2 class="section-title" @click="show.jsonBodyConditions = !show.jsonBodyConditions">-->
              <!--                  <v-icon>{{show.jsonBodyConditions ? "mdi-chevron-down" : "mdi-chevron-right"}}</v-icon>-->
              <!--                  JSON body conditions-->
              <!--                </h2>-->
              <!--              </div>-->

              <!--              <div v-if="show.jsonBodyConditions">-->
              <!--                &lt;!&ndash; JSONPath &ndash;&gt;-->
              <!--                <div class="d-flex flex-row mb-6" v-if="showBodyConditionForms">-->
              <!--                  <FormTooltip tooltipKey="jsonPath"/>-->
              <!--                  <v-textarea v-model="jsonPath" :label="formLabels.jsonPath"-->
              <!--                              :placeholder="formPlaceholderResources.jsonPath" @keyup="jsonPathChanged"/>-->
              <!--                </div>-->
              <!--              </div>-->

              <!--              <div>-->
              <!--                <h2 class="section-title" @click="show.authenticationConditions = !show.authenticationConditions">-->
              <!--                  <v-icon>{{show.authenticationConditions ? "mdi-chevron-down" : "mdi-chevron-right"}}</v-icon>-->
              <!--                  Authentication conditions-->
              <!--                </h2>-->
              <!--              </div>-->

              <!--              <div v-if="show.authenticationConditions">-->
              <!--                &lt;!&ndash; Basic authentication &ndash;&gt;-->
              <!--                <div class="d-flex flex-row mb-6">-->
              <!--                  <FormTooltip tooltipKey="fullPath"/>-->
              <!--                  <v-text-field v-model="stub.conditions.basicAuthentication.username"-->
              <!--                                :label="formLabels.basicAuthUsername" class="pa-2"/>-->
              <!--                  <v-text-field v-model="stub.conditions.basicAuthentication.password"-->
              <!--                                :label="formLabels.basicAuthPassword" class="pa-2"/>-->
              <!--                </div>-->
              <!--              </div>-->
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>

      <!--      <v-card>-->
      <!--        <v-card-title>Response writers</v-card-title>-->
      <!--        <v-card-text>-->
      <!--          <v-row>-->
      <!--            <v-col>-->

      <!--              <div>-->
      <!--                <h2 class="section-title" @click="show.generalWriters = !show.generalWriters">-->
      <!--                  <v-icon>{{show.generalWriters ? "mdi-chevron-down" : "mdi-chevron-right"}}</v-icon>-->
      <!--                  General writers-->
      <!--                </h2>-->
      <!--              </div>-->

      <!--              <div v-if="show.generalWriters">-->
      <!--                &lt;!&ndash; Status code &ndash;&gt;-->
      <!--                <div class="d-flex flex-row mb-6">-->
      <!--                  <FormTooltip tooltipKey="statusCode"/>-->
      <!--                  <v-menu absolute offset-y>-->
      <!--                    <template v-slot:activator="{on}">-->
      <!--                      <v-text-field v-model="stub.response.statusCode" :label="formLabels.statusCode" v-on="on"-->
      <!--                                    clearable-->
      <!--                                    class="pa-2"/>-->
      <!--                    </template>-->
      <!--                    <v-list max-height="300px">-->
      <!--                      <v-list-item v-for="code in formattedStatusCodes" :key="code.code"-->
      <!--                                   @click="stub.response.statusCode = code.code">-->
      <!--                        <v-list-item-title>{{code.text}}</v-list-item-title>-->
      <!--                      </v-list-item>-->
      <!--                    </v-list>-->
      <!--                  </v-menu>-->
      <!--                </div>-->

      <!--                &lt;!&ndash; Extra duration &ndash;&gt;-->
      <!--                <div class="d-flex flex-row mb-6">-->
      <!--                  <FormTooltip tooltipKey="extraDuration"/>-->
      <!--                  <v-text-field v-model="stub.response.extraDuration" :label="formLabels.extraDuration" class="pa-2"/>-->
      <!--                </div>-->

      <!--                &lt;!&ndash; Dynamic mode &ndash;&gt;-->
      <!--                <div class="d-flex flex-row mb-6">-->
      <!--                  <FormTooltip tooltipKey="dynamicMode"/>-->
      <!--                  <v-switch v-model="stub.response.enableDynamicMode" :label="formLabels.enableDynamicMode"-->
      <!--                            class="pa-2"/>-->
      <!--                </div>-->
      <!--              </div>-->

      <!--              <div>-->
      <!--                <h2 class="section-title" @click="show.bodyWriters = !show.bodyWriters">-->
      <!--                  <v-icon>{{show.bodyWriters ? "mdi-chevron-down" : "mdi-chevron-right"}}</v-icon>-->
      <!--                  Body writers-->
      <!--                </h2>-->
      <!--              </div>-->

      <!--              <div v-if="show.bodyWriters">-->
      <!--                &lt;!&ndash; Response body type &ndash;&gt;-->
      <!--                <div class="d-flex flex-row mb-6">-->
      <!--                  <FormTooltip tooltipKey="responseBodyType"/>-->
      <!--                  <v-select v-model="bodyResponseType" :items="responseBodyTypes" item-text="value" item-value="value"-->
      <!--                            :label="formLabels.responseBodyType" @change="responseBodyChanged"/>-->
      <!--                </div>-->

      <!--                &lt;!&ndash; Response body &ndash;&gt;-->
      <!--                <div class="d-flex flex-row mb-6" v-if="showResponseBodyForm">-->
      <!--                  <FormTooltip tooltipKey="responseBody"/>-->
      <!--                  <v-textarea v-model="responseBody" :label="formLabels.responseBody"-->
      <!--                              :placeholder="formPlaceholderResources.responseBody" @keyup="responseBodyChanged"-->
      <!--                              id="response-body"/>-->
      <!--                </div>-->

      <!--                &lt;!&ndash; Body variable handler &ndash;&gt;-->
      <!--                <div class="d-flex flex-row mb-6" v-if="showResponseBodyForm && dynamicModeEnabled">-->
      <!--                  <FormTooltip tooltipKey="selectVariableHandler"/>-->
      <!--                  <v-menu absolute offset-y>-->
      <!--                    <template v-slot:activator="{on}">-->
      <!--                      <v-btn color="success" v-on="on">Select variable handler</v-btn>-->
      <!--                    </template>-->
      <!--                    <v-list max-height="300px">-->
      <!--                      <v-list-item v-for="handler in variableHandlers" :key="handler.name"-->
      <!--                                   @click="insertHandlerInBody(handler)">-->
      <!--                        <v-list-item-title>{{handler.name}}: {{handler.fullName}}. Example: {{handler.example}}-->
      <!--                        </v-list-item-title>-->
      <!--                      </v-list-item>-->
      <!--                    </v-list>-->
      <!--                  </v-menu>-->
      <!--                </div>-->
      <!--              </div>-->

      <!--              <div>-->
      <!--                <h2 class="section-title" @click="show.headerWriters = !show.headerWriters">-->
      <!--                  <v-icon>{{show.headerWriters ? "mdi-chevron-down" : "mdi-chevron-right"}}</v-icon>-->
      <!--                  Header writers-->
      <!--                </h2>-->
      <!--              </div>-->

      <!--              <div v-if="show.headerWriters">-->
      <!--                &lt;!&ndash; Headers &ndash;&gt;-->
      <!--                <div class="d-flex flex-row mb-6">-->
      <!--                  <FormTooltip tooltipKey="responseHeaders"/>-->
      <!--                  <v-textarea v-model="responseHeaders" :label="formLabels.responseHeaders"-->
      <!--                              :placeholder="formPlaceholderResources.responseHeaders" @keyup="responseHeadersChanged"-->
      <!--                              id="response-headers"/>-->
      <!--                </div>-->

      <!--                &lt;!&ndash; Headers variable handler &ndash;&gt;-->
      <!--                <div class="d-flex flex-row mb-6" v-if="dynamicModeEnabled">-->
      <!--                  <FormTooltip tooltipKey="selectVariableHandler"/>-->
      <!--                  <v-menu absolute offset-y>-->
      <!--                    <template v-slot:activator="{on}">-->
      <!--                      <v-btn color="success" v-on="on">Select variable handler</v-btn>-->
      <!--                    </template>-->
      <!--                    <v-list max-height="300px">-->
      <!--                      <v-list-item v-for="handler in variableHandlers" :key="handler.name"-->
      <!--                                   @click="insertHandlerInHeaders(handler)">-->
      <!--                        <v-list-item-title>{{handler.name}}: {{handler.fullName}}. Example: {{handler.example}}-->
      <!--                        </v-list-item-title>-->
      <!--                      </v-list-item>-->
      <!--                    </v-list>-->
      <!--                  </v-menu>-->
      <!--                </div>-->
      <!--              </div>-->

      <!--              <div>-->
      <!--                <h2 class="section-title" @click="show.redirectWriters = !show.redirectWriters">-->
      <!--                  <v-icon>{{show.redirectWriters ? "mdi-chevron-down" : "mdi-chevron-right"}}</v-icon>-->
      <!--                  Redirection writers-->
      <!--                </h2>-->
      <!--              </div>-->

      <!--              <div v-if="show.redirectWriters">-->
      <!--                <div class="d-flex flex-row mb-6">-->
      <!--                  <FormTooltip tooltipKey="redirect"/>-->
      <!--                  <v-text-field v-model="stub.response.temporaryRedirect" :label="formLabels.temporaryRedirect"-->
      <!--                                class="pa-2"-->
      <!--                                :placeholder="formPlaceholderResources.redirect"/>-->
      <!--                </div>-->

      <!--                <div class="d-flex flex-row mb-6">-->
      <!--                  <FormTooltip tooltipKey="redirect"/>-->
      <!--                  <v-text-field v-model="stub.response.permanentRedirect" :label="formLabels.permanentRedirect"-->
      <!--                                class="pa-2"-->
      <!--                                :placeholder="formPlaceholderResources.redirect"/>-->
      <!--                </div>-->
      <!--              </div>-->
      <!--            </v-col>-->
      <!--          </v-row>-->
      <!--        </v-card-text>-->
      <!--      </v-card>-->
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
      stubId() {
        // Get ID from route for editing
        return this.$route.params.id;
      },
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
        return this.bodyResponseType !== responseBodyTypes.empty;
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
        stubXPathNamespaces: "stubForm.xpathNamespaces"
      })
    },
    methods: {
      async initialize() {
        this.tenantNames = await this.$store.dispatch(
          actionNames.getTenantNames
        );
      },
      validateForm() {
        const validationMessages = [];
        if (this.queryStrings && !this.stub.conditions.url.query) {
          validationMessages.push(formValidationMessages.queryStringIncorrect);
        }

        if (this.formBody && !this.stub.conditions.form) {
          validationMessages.push(formValidationMessages.formBodyIncorrect);
        }

        if (this.headers && !this.stub.conditions.headers) {
          validationMessages.push(formValidationMessages.headersIncorrect);
        }

        if (isNaN(this.stub.priority)) {
          validationMessages.push(formValidationMessages.priorityNotInteger);
        }

        if (this.xpathNamespaces && !this.stub.conditions.xpath) {
          validationMessages.push(formValidationMessages.xpathNotFilledIn);
        }

        if (
          !this.stub.conditions.basicAuthentication.username && this.stub.conditions.basicAuthentication.password ||
          this.stub.conditions.basicAuthentication.username && !this.stub.conditions.basicAuthentication.password) {
          validationMessages.push(formValidationMessages.basicAuthInvalid);
        }

        const statusCode = this.stub.response.statusCode;
        const parsedStatusCode = parseInt(statusCode);
        if (statusCode !== null && (isNaN(parsedStatusCode) || parsedStatusCode < 100 || parsedStatusCode >= 600)) {
          validationMessages.push(formValidationMessages.fillInCorrectStatusCode);
        }

        if (this.responseHeaders && !this.stub.response.headers) {
          validationMessages.push(formValidationMessages.headersIncorrect);
        }

        const extraDuration = this.stub.response.extraDuration;
        const parsedExtraDuration = parseInt(extraDuration);
        if (extraDuration !== null && (isNaN(parsedExtraDuration) || parsedExtraDuration <= 0)) {
          validationMessages.push(formValidationMessages.extraDurationInvalid);
        }

        if (this.stub.response.permanentRedirect && this.stub.response.temporaryRedirect) {
          validationMessages.push(formValidationMessages.fillInOneTypeOfRedirect);
        }

        return validationMessages;
      },
      async saveStub() {
        const messages = this.validateForm();
        if (messages.length) {
          for (let message of messages) {
            toastError(message);
          }

          return;
        }

        try {
          const results = await this.$store.dispatch(actionNames.addStubs, {
            input: this.stub,
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
      },
      jsonPathChanged() {
        const result = this.parseLines(this.jsonPath);
        this.stub.conditions.jsonPath = result.length ? result : null;
      },
      responseBodyChanged() {
        this.stub.response.html = null;
        this.stub.response.text = null;
        this.stub.response.json = null;
        this.stub.response.xml = null;
        this.stub.response.base64 = null;
        switch (this.bodyResponseType) {
          case responseBodyTypes.custom:
          case responseBodyTypes.text:
            this.stub.response.text = this.responseBody;
            break;
          case responseBodyTypes.html:
            this.stub.response.html = this.responseBody;
            break;
          case responseBodyTypes.json:
            this.stub.response.json = this.responseBody;
            break;
          case responseBodyTypes.xml:
            this.stub.response.xml = this.responseBody;
            break;
          case responseBodyTypes.base64:
            this.stub.response.base64 = this.responseBody;
            break;
        }
      },
      responseHeadersChanged() {
        const result = this.parseKeyValue(this.responseHeaders);
        this.stub.response.headers = Object.keys(result).length ? result : null;
      },
      insertHandlerInBody(handler) {
        this.responseBody = this.insertHandler(handler, this.responseBody || "", "response-body");
      },
      insertHandlerInHeaders(handler) {
        this.responseHeaders = this.insertHandler(handler, this.responseHeaders || "", "response-headers");
      },
      insertHandler(handler, text, elementId) {
        this.stub.response.enableDynamicMode = true;
        const elem = document.getElementById(elementId);
        const position = elem.selectionStart || 0;
        return [text.slice(0, position), handler.example, text.slice(position)].join("");
      }
    },
    watch: {
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
