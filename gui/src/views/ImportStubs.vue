<template>
  <div>
    <h1>Import stubs</h1>
    <div class="col-md-12 mb-3">
      <button
        v-for="tab of tabs"
        :key="tab"
        class="btn me-2"
        :class="{
          'btn-outline-success': selectedTab !== tab,
          'btn-success': selectedTab === tab,
        }"
        @click="changeTab(tab)"
      >
        {{ tabNames[tab] }}
      </button>
    </div>
    <div class="col-md-12 mt-3" v-if="selectedTab === tabs.uploadStubs">
      <UploadStubs />
    </div>
    <div class="col-md-12 mt-3" v-if="selectedTab === tabs.importCurl">
      <ImportCurl />
    </div>
    <div class="col-md-12 mt-3" v-if="selectedTab === tabs.importHar">
      <ImportHar />
    </div>
    <div class="col-md-12 mt-3" v-if="selectedTab === tabs.importOpenApi">
      <ImportOpenApi />
    </div>
  </div>
</template>

<script>
import UploadStubs from "@/components/stub/UploadStubs";
import ImportCurl from "@/components/stub/ImportCurl";
import ImportHar from "@/components/stub/ImportHar";
import ImportOpenApi from "@/components/stub/ImportOpenApi";
import { ref } from "vue";
import { useRoute, useRouter } from "vue-router";

const tabs = {
  uploadStubs: "uploadStubs",
  importCurl: "importCurl",
  importHar: "importHar",
  importOpenApi: "importOpenApi",
};

const tabNames = {
  uploadStubs: "Upload stubs",
  importCurl: "Import cURL command(s)",
  importHar: "Import HTTP archive (HAR)",
  importOpenApi: "Import OpenAPI definition",
};

export default {
  name: "ImportStubs",
  components: { ImportOpenApi, UploadStubs, ImportCurl, ImportHar },
  setup() {
    const router = useRouter();
    const route = useRoute();

    // Data
    const selectedTab = ref(route.query.tab || tabs.uploadStubs);

    // Methods
    const changeTab = async (tab) => {
      selectedTab.value = tab;
      await router.push({ name: "ImportStubs", query: { tab } });
    };

    return { tabs, tabNames, selectedTab, changeTab };
  },
};
</script>

<style scoped></style>
