<template>
  <v-expansion-panel>
    <v-expansion-panel-header>{{ fullStub.stub.id }}</v-expansion-panel-header>
    <v-expansion-panel-content>
      <v-row>
        <v-col class="buttons">
          <v-btn
            color="success"
            title="View all requests made for this stub"
            :to="{name: routeNames.requests,query: {searchTerm: this.fullStub.stub.id}}"
          >View requests
          </v-btn
          >
          <v-btn
            color="success"
            v-if="!fullStub.metadata.readOnly"
            :to="{name: routeNames.updateStub,params: { stubId: this.fullStub.stub.id }}"
          >Update stub as YAML
          </v-btn
          >
          <v-btn
            color="success"
            v-if="!fullStub.metadata.readOnly"
            :to="{name: routeNames.stubForm,params: {id: this.fullStub.stub.id}}"
          >Update stub with form
          </v-btn
          >
          <v-btn
            color="error"
            @click.stop="deleteDialog = true"
            v-if="!fullStub.metadata.readOnly"
          >Delete stub
          </v-btn
          >
        </v-col>
      </v-row>
      <pre>{{ fullStub.stub | yaml }}</pre>
    </v-expansion-panel-content>
    <v-dialog v-model="deleteDialog" max-width="290">
      <v-card>
        <v-card-title class="headline">Delete the stub?</v-card-title>
        <v-card-text>The stub can't be recovered.</v-card-text>
        <v-card-actions>
          <div class="flex-grow-1"></div>
          <v-btn color="green darken-1" text @click="deleteDialog = false"
          >No
          </v-btn
          >
          <v-btn color="green darken-1" text @click="deleteStub">Yes</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-expansion-panel>
</template>

<script>
  import {toastSuccess} from "@/utils/toastUtil";
  import {resources} from "@/shared/resources";
  import {routeNames} from "@/router/routerConstants";
  import {actionNames} from "@/store/storeConstants";

  export default {
    name: "stub",
    props: ["fullStub"],
    data() {
      return {
        deleteDialog: false,
        routeNames
      };
    },
    methods: {
      async deleteStub() {
        this.deleteDialog = false;
        const stubId = this.fullStub.stub.id;
        await this.$store.dispatch(actionNames.deleteStub, {stubId});
        toastSuccess(resources.stubDeletedSuccessfully.format(stubId));
        this.$emit("deleted", this.fullStub);
      }
    }
  };
</script>

<style scoped>
  .buttons > button, .buttons > a {
    margin-right: 10px;
    margin-top: 10px;
  }
</style>
