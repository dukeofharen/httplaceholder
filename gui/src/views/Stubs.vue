<template>
  <v-row>
    <v-col>
      <h1>Stubs</h1>
      <v-row>
        <v-col class="buttons">
          <v-btn title="Refresh" @click="getStubs" color="success">Refresh</v-btn>
          <v-btn
            title="Delete all stubs"
            @click="deleteAllDialog = true"
            color="error"
          >Delete all stubs</v-btn>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <v-text-field v-model="searchTerm" placeholder="Filter on stub ID or tenant..." clearable></v-text-field>
          <v-select
            :items="tenantNames"
            placeholder="Select stub tenant / category name for the stubs you would like to see the stubs for..."
            v-model="selectedTenantName"
            clearable
          ></v-select>
        </v-col>
        <v-expansion-panels>
          <Stub v-bind:fullStub="stub" v-for="stub in filteredStubs" :key="stub.id"></Stub>
        </v-expansion-panels>
      </v-row>
    </v-col>
    <v-dialog v-model="deleteAllDialog" max-width="290">
      <v-card>
        <v-card-title class="headline">Delete all stubs?</v-card-title>
        <v-card-text>The stubs can't be recovered.</v-card-text>
        <v-card-actions>
          <div class="flex-grow-1"></div>
          <v-btn color="green darken-1" text @click="deleteAllDialog = false">No</v-btn>
          <v-btn color="green darken-1" text @click="deleteAllStubs">Yes</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-row>
</template>

<script>
import Stub from "@/components/Stub";
import { resources } from "@/resources";

export default {
  name: "stubs",
  data() {
    return {
      filteredStubs: [],
      selectedTenantName: "",
      searchTerm: "",
      deleteAllDialog: false
    };
  },
  components: {
    Stub
  },
  created() {
    this.getStubs();
    this.getTenantNames();
  },
  methods: {
    search(newValue) {
      if (!newValue) {
        this.filteredStubs = this.stubs;
      } else {
        this.filteredStubs = this.stubs.filter(r => {
          return (
            r.stub.id.includes(newValue) ||
            (r.stub.tenant && r.stub.tenant.includes(newValue))
          );
        });
      }
    },
    handleUrlSearch() {
      let term = this.$route.query.searchTerm;
      if (term) {
        this.searchTerm = term;
      }

      let tenant = this.$route.query.stubTenant;
      if (tenant) {
        this.selectedTenantName = tenant;
      }
    },
    getStubs() {
      this.$store.dispatch("getStubs");
    },
    getTenantNames() {
      this.$store.dispatch("getTenantNames");
    },
    addStub() {
      this.$router.push({ name: "addStub" });
    },
    downloadStubs() {
      this.$router.push({ name: "downloadStubs" });
    },
    clearInput() {
      this.searchTerm = "";
    },
    deleteAllStubs() {
      this.deleteAllDialog = false;
      this.$store.dispatch("deleteAllStubs");
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
    searchTerm(newValue, oldValue) {
      this.search(newValue);
    },
    $route() {
      this.handleUrlSearch();
    },
    stubs(newStubs) {
      this.filteredStubs = newStubs;
      this.handleUrlSearch();
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

<style scoped>
.buttons > button {
  margin-right: 10px;
  margin-top: 10px;
}
</style>