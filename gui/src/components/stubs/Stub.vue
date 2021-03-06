<template>
  <v-expansion-panel>
    <v-expansion-panel-header
      @click="loadStub"
      :class="{ disabled: !overviewStub.stub.enabled }"
      >{{ overviewStub.stub.id
      }}<span v-if="!overviewStub.stub.enabled"
        >&nbsp;(disabled)</span
      ></v-expansion-panel-header
    >
    <v-expansion-panel-content v-if="fullStub">
      <v-row>
        <v-col class="buttons">
          <v-btn
            color="success"
            title="View all requests made for this stub"
            :to="{
              name: routeNames.requests,
              query: { searchTerm: this.fullStub.stub.id }
            }"
            >View requests
          </v-btn>
          <v-btn
            color="success"
            v-if="!fullStub.metadata.readOnly"
            :to="{
              name: routeNames.stubForm,
              params: { stubId: this.fullStub.stub.id }
            }"
            >Update stub
          </v-btn>
          <v-btn
            color="success"
            v-if="!fullStub.metadata.readOnly"
            @click="enableOrDisableStub()"
            >{{ overviewStub.stub.enabled ? "Disable stub" : "Enable stub" }}
          </v-btn>
          <v-btn
            color="error"
            @click.stop="deleteDialog = true"
            v-if="!fullStub.metadata.readOnly"
            >Delete stub
          </v-btn>
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
          </v-btn>
          <v-btn color="green darken-1" text @click="deleteStub">Yes</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-expansion-panel>
</template>

<script>
import { toastSuccess } from "@/utils/toastUtil";
import { resources } from "@/shared/resources";
import { routeNames } from "@/router/routerConstants";

export default {
  name: "stub",
  props: ["overviewStub"],
  data() {
    return {
      deleteDialog: false,
      routeNames,
      fullStub: null
    };
  },
  methods: {
    async deleteStub() {
      this.deleteDialog = false;
      const stubId = this.overviewStub.stub.id;
      await this.$store.dispatch("stubs/deleteStub", { stubId });
      toastSuccess(resources.stubDeletedSuccessfully.format(stubId));
      this.$emit("updated", this.fullStub);
    },
    async loadStub(forceLoad) {
      if (!this.fullStub || forceLoad) {
        this.fullStub = await this.$store.dispatch("stubs/getStub", {
          stubId: this.overviewStub.stub.id
        });
      }
    },
    async enableOrDisableStub() {
      const stubId = this.overviewStub.stub.id;
      if (this.overviewStub.stub.enabled) {
        await this.$store.dispatch("stubs/disableStub", stubId);
      } else {
        await this.$store.dispatch("stubs/enableStub", stubId);
      }

      this.$emit("updated", this.fullStub);
      await this.loadStub(true);
    }
  }
};
</script>

<style scoped>
.buttons > button,
.buttons > a {
  margin-right: 10px;
  margin-top: 10px;
}

.disabled {
  color: #969696;
}
</style>
