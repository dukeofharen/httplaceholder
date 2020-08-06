<template>
  <v-row v-shortkey="['ctrl', 's']" @shortkey="addStub">
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
                <v-text-field v-model="stub.id" :label="formLabels.id" class="pa-2"/>
              </div>

              <!-- Tenant -->
              <div class="d-flex flex-row mb-6">
                <FormTooltip tooltipKey="tenant"/>
                <v-menu absolute offset-y>
                  <template v-slot:activator="{on}">
                    <v-text-field v-model="stub.tenant" :label="formLabels.tenant" v-on="on" clearable
                                  class="pa-2"/>
                  </template>
                  <v-list>
                    <v-list-item v-for="(tenant, index) in filteredTenantNames" :key="index"
                                 @click="stub.tenant = tenant">
                      <v-list-item-title>{{tenant}}</v-list-item-title>
                    </v-list-item>
                  </v-list>
                </v-menu>
              </div>

              <!-- Description -->
              <div class="d-flex flex-row mb-6">
                <FormTooltip tooltipKey="description"/>
                <v-textarea v-model="stub.description" :label="formLabels.description"/>
              </div>

              <!-- Priority -->
              <div class="d-flex flex-row mb-6">
                <FormTooltip tooltipKey="priority"/>
                <v-text-field v-model="stub.priority" :label="formLabels.priority" class="pa-2"/>
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
                      <v-text-field v-model="stub.conditions.method" :label="formLabels.httpMethod" v-on="on"
                                    clearable
                                    class="pa-2"/>
                    </template>
                    <v-list>
                      <v-list-item v-for="(method, index) in httpMethods" :key="index"
                                   @click="stub.conditions.method = method">
                        <v-list-item-title>{{method}}</v-list-item-title>
                      </v-list-item>
                    </v-list>
                  </v-menu>
                </div>

                <!-- Client IP -->
                <div class="d-flex flex-row mb-6">
                  <FormTooltip tooltipKey="clientIp"/>
                  <v-text-field v-model="stub.conditions.clientIp" :label="formLabels.clientIp" class="pa-2"
                                :placeholder="formPlaceholderResources.clientIp"/>
                </div>

                <!-- Hostname -->
                <div class="d-flex flex-row mb-6">
                  <FormTooltip tooltipKey="hostname"/>
                  <v-text-field v-model="stub.conditions.hostname" :label="formLabels.hostname" class="pa-2"
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
                  <v-text-field v-model="stub.conditions.url.path" :label="formLabels.urlPath" class="pa-2"
                                :placeholder="formPlaceholderResources.urlPath"/>
                </div>

                <!-- Query string -->
                <div class="d-flex flex-row mb-6">
                  <FormTooltip tooltipKey="queryString"/>
                  <v-textarea v-model="queryStrings" :label="formLabels.queryString"
                              :placeholder="formPlaceholderResources.queryString" @keyup="queryStringChanged"/>
                </div>

                <!-- Full path -->
                <div class="d-flex flex-row mb-6">
                  <FormTooltip tooltipKey="fullPath"/>
                  <v-text-field v-model="stub.conditions.url.fullPath" :label="formLabels.fullPath" class="pa-2"
                                :placeholder="formPlaceholderResources.fullPath"/>
                </div>

                <!-- Is HTTPS -->
                <div class="d-flex flex-row mb-6">
                  <FormTooltip tooltipKey="isHttps"/>
                  <v-radio-group v-model="isHttps" class="pa-2" @change="isHttpsSelected">
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
                  <v-textarea v-model="headers" :label="formLabels.headers"
                              :placeholder="formPlaceholderResources.headers" @keyup="headersChanged"/>
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
                  <v-textarea v-model="body" :label="formLabels.body"
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
                  <v-textarea v-model="formBody" :label="formLabels.formBody"
                              :placeholder="formPlaceholderResources.formBody" @keyup="formBodyChanged"/>
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
                  <v-textarea v-model="xpath" :label="formLabels.xpath"
                              :placeholder="formPlaceholderResources.xpath" @keyup="xpathChanged"/>
                </div>

                <div class="d-flex flex-row mb-6" v-if="showBodyConditionForms">
                  <FormTooltip tooltipKey="xpathNamespaces"/>
                  <v-textarea v-model="xpathNamespaces" :label="formLabels.xpathNamespaces"
                              :placeholder="formPlaceholderResources.xpathNamespaces" @keyup="xpathChanged"/>
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
                  <v-textarea v-model="jsonPath" :label="formLabels.jsonPath"
                              :placeholder="formPlaceholderResources.jsonPath" @keyup="jsonPathChanged"/>
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
                  <v-text-field v-model="stub.conditions.basicAuthentication.username"
                                :label="formLabels.basicAuthUsername" class="pa-2"/>
                  <v-text-field v-model="stub.conditions.basicAuthentication.password"
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
                      <v-text-field v-model="stub.response.statusCode" :label="formLabels.statusCode" v-on="on"
                                    clearable
                                    class="pa-2"/>
                    </template>
                    <v-list max-height="300px">
                      <v-list-item v-for="code in formattedStatusCodes" :key="code.code"
                                   @click="stub.response.statusCode = code.code">
                        <v-list-item-title>{{code.text}}</v-list-item-title>
                      </v-list-item>
                    </v-list>
                  </v-menu>
                </div>

                <!-- Extra duration -->
                <div class="d-flex flex-row mb-6">
                  <FormTooltip tooltipKey="extraDuration"/>
                  <v-text-field v-model="stub.response.extraDuration" :label="formLabels.extraDuration" class="pa-2"/>
                </div>
              </div>

              <div>
                <h2 class="section-title" @click="show.bodyWriters = !show.bodyWriters">
                  <v-icon>{{show.bodyWriters ? "mdi-chevron-down" : "mdi-chevron-right"}}</v-icon>
                  Body writers
                </h2>
              </div>

              <div v-if="show.bodyWriters">
                <!-- Select response body type -->
                <div class="d-flex flex-row mb-6">
                  <FormTooltip tooltipKey="responseBodyType"/>
                  <v-select v-model="bodyResponseType" :items="responseBodyTypes" item-text="value" item-value="value"
                            :label="formLabels.responseBodyType" @change="responseBodyChanged"/>
                </div>

                <!-- Response body -->
                <div class="d-flex flex-row mb-6" v-if="showResponseBodyForm">
                  <FormTooltip tooltipKey="responseBody"/>
                  <v-textarea v-model="responseBody" :label="formLabels.responseBody"
                              :placeholder="formPlaceholderResources.responseBody" @keyup="responseBodyChanged"/>
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
                  <v-textarea v-model="responseHeaders" :label="formLabels.responseHeaders"
                              :placeholder="formPlaceholderResources.responseHeaders" @keyup="responseHeadersChanged"/>
                </div>
              </div>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
      <v-btn color="success" @click="addStub">Add stub</v-btn>
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

  export default {
    name: "addStubForm",
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
          headerWriters: false
        },
        tenantNames: [],
        httpMethods,
        formPlaceholderResources,
        formLabels,
        isHttpsValues,
        queryStrings: "",
        body: "",
        formBody: "",
        xpath: "",
        xpathNamespaces: "",
        jsonPath: "",
        headers: "",
        isHttps: isHttpsValues.httpAndHttps,
        bodyResponseType: responseBodyTypes.text,
        responseBody: null,
        responseHeaders: "",
        stub: {
          id: null,
          tenant: null,
          description: null,
          priority: 0,
          conditions: {
            method: null,
            url: {
              path: null,
              query: null,
              fullPath: null,
              isHttps: null
            },
            body: null,
            form: null,
            xpath: null,
            jsonPath: null,
            basicAuthentication: {
              username: null,
              password: null
            },
            clientIp: null,
            hostname: null,
            headers: null
          },
          response: {
            statusCode: 200,
            text: null,
            json: null,
            html: null,
            xml: null,
            base64: null,
            headers: null,
            extraDuration: null
          }
        }
      };
    },
    computed: {
      filteredTenantNames() {
        if (!this.stub.tenant) {
          return this.tenantNames;
        }

        return this.tenantNames.filter(t => t.includes(this.stub.tenant));
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
        return this.stub.conditions.method !== "GET";
      }
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

        const parsedStatusCode = parseInt(this.stub.response.statusCode);
        if (isNaN(parsedStatusCode) || parsedStatusCode < 100 || parsedStatusCode >= 600) {
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

        return validationMessages;
      },
      async addStub() {
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
      queryStringChanged() {
        const result = this.parseKeyValue(this.queryStrings);
        if (!Object.keys(result).length) {
          this.stub.conditions.url.query = null;
        } else {
          this.stub.conditions.url.query = result;
        }
      },
      bodyChanged() {
        const result = this.parseLines(this.body);
        this.stub.conditions.body = result.length ? result : null;
      },
      formBodyChanged() {
        const result = this.parseKeyValue(this.formBody);
        const keys = Object.keys(result);
        if (!keys.length) {
          this.stub.conditions.form = null;
        } else {
          this.stub.conditions.form = keys.map(k => ({key: k, value: result[k]}));
        }
      },
      xpathChanged() {
        const result = this.parseLines(this.xpath);
        if (!result.length) {
          this.stub.conditions.xpath = null;
        } else {
          this.stub.conditions.xpath = result.map(e => ({queryString: e}));
          const nsResult = this.parseKeyValue(this.xpathNamespaces);
          const nsKeys = Object.keys(nsResult);
          let namespaces = {};
          if (nsKeys.length) {
            for (let key of nsKeys) {
              namespaces[key] = nsResult[key];
            }
          } else {
            namespaces = null;
          }

          for (let expression of this.stub.conditions.xpath) {
            expression.namespaces = namespaces;
          }
        }
      },
      jsonPathChanged() {
        const result = this.parseLines(this.jsonPath);
        this.stub.conditions.jsonPath = result.length ? result : null;
      },
      headersChanged() {
        const result = this.parseKeyValue(this.headers);
        this.stub.conditions.headers = Object.keys(result).length ? result : null;
      },
      parseLines(input) {
        const result = input.split(/\r?\n/);
        if (result.every(l => !l)) {
          return [];
        }

        return result;
      },
      parseKeyValue(input) {
        let result = {};
        const lines = input.split(/\r?\n/);

        for (let line of lines) {
          let parts = line.split(":");
          if (parts.length <= 1) {
            continue;
          }

          let key = parts[0];
          let value = parts.slice(1).join(":").trim();
          result[key] = value;
        }

        return result;
      },
      isHttpsSelected() {
        // I had to add this intermediate function, because for some reason, Vuetify doesn't allow the binding of a "null" value.
        switch (this.isHttps) {
          case isHttpsValues.httpAndHttps:
            this.stub.conditions.url.isHttps = null;
            break;
          case isHttpsValues.onlyHttp:
            this.stub.conditions.url.isHttps = false;
            break;
          case isHttpsValues.onlyHttps:
            this.stub.conditions.url.isHttps = true;
            break;
        }
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

          if (this.stub.response.extraDuration === "") {
            this.stub.response.extraDuration = null;
          }

          console.log(JSON.stringify(this.stub)); // TODO remove this
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
