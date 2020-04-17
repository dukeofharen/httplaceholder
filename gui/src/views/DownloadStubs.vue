<template>
  <v-row>
    <v-col>
      <h1>Download all stubs</h1>
      <v-card>
        <v-card-text
          >This page displays all stubs currently present in HttPlaceholder. You
          can copy this string and put it in a .yml file on your PC for local
          development or directly download the file.</v-card-text
        >
      </v-card>
      <v-row>
        <v-col>
          <v-card>
            <v-card-actions>
              <v-select
                :items="tenantNames"
                placeholder="Select stub tenant / category name for the stubs you would like to download..."
                v-model="selectedTenantName"
                clearable
              ></v-select>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <v-card>
            <v-card-actions>
              <v-textarea v-model="downloadString"></v-textarea>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <v-btn color="success" @click="downloadStubs">Download stubs</v-btn>
        </v-col>
      </v-row>
    </v-col>
  </v-row>
</template>

<script>
import yaml from "js-yaml";
import { resources } from "@/shared/resources";
import { downloadBlob } from "@/utils/downloadHelper";

export default {
  name: "addStub",
  created() {
    this.$store.dispatch("getStubs");
    this.$store.dispatch("getTenantNames");
  },
  data() {
    return {
      downloadString: "",
      selectedTenantName: "",
      filteredStubs: []
    };
  },
  methods: {
    downloadStubs() {
      downloadBlob("stubs.yml", this.downloadString);
    }
  },
  computed: {
    stubs() {
      return this.$store.getters.getStubs;
    },
    tenantNames() {
      return this.$store.getters.getTenantNames;
    }
  },
  watch: {
    stubs(newStubs) {
      this.filteredStubs = newStubs;
    },
    filteredStubs(newStubs) {
      let stubsForDownload = newStubs.map(fullStub => {
        return fullStub.stub;
      });
      this.downloadString =
        resources.downloadStubsHeader + "\n" + yaml.dump(stubsForDownload);
    },
    selectedTenantName(val) {
      if (!val) {
        this.filteredStubs = this.stubs;
      } else {
        this.filteredStubs = this.stubs.filter(
          stub => stub.stub.tenant === val
        );
      }
    }
  }
};
</script>

<style scoped></style>
