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
              <v-text-field v-model="stub.id" label="ID"/>
              <v-menu absolute offset-y>
                <template v-slot:activator="{on}">
                  <v-text-field v-model="stub.tenant" label="Stub tenant / category" v-on="on" clearable/>
                </template>
                <v-list>
                  <v-list-item v-for="(tenant, index) in filteredTenantNames" :key="index" @click="tenantSelect(tenant)" re>
                    <v-list-item-title>{{tenant}}</v-list-item-title>
                  </v-list-item>
                </v-list>
              </v-menu>
              <v-textarea v-model="stub.description" label="Description" />
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
      <v-card>
        <v-card-title>Conditions</v-card-title>
      </v-card>
      <v-btn color="success" @click="addStub">Add stub</v-btn>
    </v-col>
  </v-row>
</template>

<script>
  import {actionNames} from "@/store/storeConstants";

  export default {
    name: "addStubForm",
    async mounted() {
      await this.initialize();
    },
    data() {
      return {
        tenantNames: [],
        stub: {
          id: "",
          tenant: "",
          description: "",
          priority: 0
        }
      };
    },
    computed: {
      filteredTenantNames() {
        if (!this.stub.tenant) {
          return this.tenantNames;
        }

        return this.tenantNames.filter(t => t.includes(this.stub.tenant));
      }
    },
    methods: {
      async initialize() {
        this.tenantNames = await this.$store.dispatch(
          actionNames.getTenantNames
        );
      },
      addStub() {
        console.log(JSON.stringify(this.stub));
      },
      tenantSelect(tenant) {
        this.stub.tenant = tenant;
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
