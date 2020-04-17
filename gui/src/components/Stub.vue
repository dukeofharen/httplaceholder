<template>
  <v-expansion-panel>
    <v-expansion-panel-header>{{ fullStub.stub.id }}</v-expansion-panel-header>
    <v-expansion-panel-content>
      <v-row>
        <v-col class="buttons">
          <v-btn
            color="success"
            @click="viewRequests"
            title="View all requests made for this stub"
            >View requests</v-btn
          >
          <v-btn
            color="success"
            @click="updateStub"
            v-if="!fullStub.metadata.readOnly"
            >Update stub</v-btn
          >
          <v-btn
            color="error"
            @click.stop="deleteDialog = true"
            v-if="!fullStub.metadata.readOnly"
            >Delete stub</v-btn
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
            >No</v-btn
          >
          <v-btn color="green darken-1" text @click="deleteStub">Yes</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-expansion-panel>
</template>

<script>
export default {
  name: "stub",
  props: ["fullStub"],
  data() {
    return {
      deleteDialog: false
    };
  },
  created() {},
  methods: {
    deleteStub() {
      this.deleteDialog = false;
      this.$store.dispatch("deleteStub", { stubId: this.fullStub.stub.id });
    },
    updateStub() {
      this.$router.push({
        name: "updateStub",
        params: { stubId: this.fullStub.stub.id }
      });
    },
    viewRequests() {
      this.$router.push({
        name: "requests",
        query: { searchTerm: this.fullStub.stub.id }
      });
    }
  }
};
</script>

<style scoped>
.buttons > button {
  margin-right: 10px;
  margin-top: 10px;
}
</style>
