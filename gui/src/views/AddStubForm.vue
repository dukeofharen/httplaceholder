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
                <v-text-field v-model="stub.id" label="ID" class="pa-2"/>
              </div>

              <!-- Tenant -->
              <div class="d-flex flex-row mb-6">
                <FormTooltip tooltipKey="tenant"/>
                <v-menu absolute offset-y>
                  <template v-slot:activator="{on}">
                    <v-text-field v-model="stub.tenant" label="Stub tenant / category" v-on="on" clearable
                                  class="pa-2"/>
                  </template>
                  <v-list>
                    <v-list-item v-for="(tenant, index) in filteredTenantNames" :key="index"
                                 @click="tenantSelect(tenant)">
                      <v-list-item-title>{{tenant}}</v-list-item-title>
                    </v-list-item>
                  </v-list>
                </v-menu>
              </div>

              <!-- Description -->
              <div class="d-flex flex-row mb-6">
                <FormTooltip tooltipKey="description"/>
                <v-textarea v-model="stub.description" label="Description"/>
              </div>

              <!-- Priority -->
              <div class="d-flex flex-row mb-6">
                <FormTooltip tooltipKey="priority"/>
                <v-text-field v-model="stub.priority" label="Priority" class="pa-2"/>
              </div>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
      <v-card>
        <v-card-title>Conditions</v-card-title>
        <v-card-text>
          <v-row>
            <v-col>
              <div>
                <h2>General conditions</h2>
              </div>

              <!-- HTTP method -->
              <div class="d-flex flex-row mb-6">
                <FormTooltip tooltipKey="httpMethod"/>
                <v-menu absolute offset-y>
                  <template v-slot:activator="{on}">
                    <v-text-field v-model="stub.conditions.method" label="HTTP method" v-on="on" clearable
                                  class="pa-2"/>
                  </template>
                  <v-list>
                    <v-list-item v-for="(method, index) in httpMethods" :key="index"
                                 @click="methodSelect(method)">
                      <v-list-item-title>{{method}}</v-list-item-title>
                    </v-list-item>
                  </v-list>
                </v-menu>
              </div>

              <div>
                <h2>URL conditions</h2>
              </div>

              <!-- URL path -->
              <div class="d-flex flex-row mb-6">
                <FormTooltip tooltipKey="urlPath"/>
                <v-text-field v-model="stub.conditions.url.path" label="URL path" class="pa-2"
                              :placeholder="formPlaceholderResources.urlPath"/>
              </div>

              <!-- Query string -->
              <div class="d-flex flex-row mb-6">
                <FormTooltip tooltipKey="queryString"/>
                <v-textarea v-model="queryStrings" label="Query strings (1 on each line)"
                            :placeholder="formPlaceholderResources.queryString" @keyup="queryStringChanged"/>
              </div>

              <!-- Full path -->
              <div class="d-flex flex-row mb-6">
                <FormTooltip tooltipKey="fullPath"/>
                <v-text-field v-model="stub.conditions.url.fullPath" label="Full path" class="pa-2"
                              :placeholder="formPlaceholderResources.fullPath"/>
              </div>

              <!-- Is HTTPS -->
              <div class="d-flex flex-row mb-6">
                <FormTooltip tooltipKey="isHttps"/>
                <v-switch v-model="stub.conditions.url.isHttps" class="pa-2" :label="isHttpsLabel"/>
              </div>

              <div>
                <v-btn color="primary" @click="stub.conditions.url.isHttps = null">Clear HTTPS bit</v-btn>
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
  import {httpMethods} from "@/shared/resources";
  import FormTooltip from "@/components/FormTooltip";
  import {toastError, toastSuccess} from "@/utils/toastUtil";
  import {resources} from "@/shared/resources";
  import {formPlaceholderResources, formValidationMessages, formLabels} from "@/shared/stubFormResources";

  export default {
    name: "addStubForm",
    components: {FormTooltip},
    async mounted() {
      await this.initialize();
    },
    data() {
      return {
        tenantNames: [],
        httpMethods,
        formPlaceholderResources,
        queryStrings: "",
        stub: {
          id: "",
          tenant: "",
          description: "",
          priority: 0,
          conditions: {
            method: null,
            url: {
              path: null,
              query: null,
              fullPath: null,
              isHttps: null
            }
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
      isHttpsLabel() {
        const httpsStatus = this.stub.conditions.url.isHttps;
        if(httpsStatus === true) {
          return formLabels.isHttpsTrue;
        } else if (httpsStatus === false) {
          return formLabels.isHttpsFalse;
        }

        return formLabels.isHttpsNull;
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

        console.log(this.stub.priority);
        if (isNaN(this.stub.priority)) {
          validationMessages.push(formValidationMessages.priorityNotInteger);
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
      tenantSelect(tenant) {
        this.stub.tenant = tenant;
      },
      methodSelect(method) {
        this.stub.conditions.method = method;
      },
      queryStringChanged() {
        const value = this.parseKeyValue(this.queryStrings);
        if (!Object.keys(value).length) {
          this.stub.conditions.url.query = null;
        } else {
          this.stub.conditions.url.query = value;
        }
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
      }
    }
  };
</script>

<style scoped>
  .v-card {
    margin-top: 10px;
    margin-bottom: 10px;
  }
</style>
