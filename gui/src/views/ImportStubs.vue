<template>
  <div>
    <h1>Import stubs</h1>
    <div class="col-md-12 mb-3">
      <button
        v-for="tab of tabs"
        :key="tab"
        class="btn me-2 tab-button btn-mobile full-width"
        :class="{
          'btn-outline-success': selectedTab !== tab,
          'btn-success': selectedTab === tab,
        }"
        @click="changeTab(tab)"
      >
        <i
          v-if="tabDetails[tab].icon"
          class="bi"
          :class="tabDetails[tab].icon"
        />
        {{ tabDetails[tab].title }}
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

<script lang="ts">
import UploadStubs from "@/components/import/UploadStubs.vue";
import ImportCurl from "@/components/import/ImportCurl.vue";
import ImportHar from "@/components/import/ImportHar.vue";
import ImportOpenApi from "@/components/import/ImportOpenApi.vue";
import { ref } from "vue";
import { useRoute, useRouter } from "vue-router";
import { defineComponent } from "vue";

const tabs = {
  uploadStubs: "uploadStubs",
  importCurl: "importCurl",
  importHar: "importHar",
  importOpenApi: "importOpenApi",
};

const tabDetails: any = {
  uploadStubs: {
    title: "Upload stubs",
    icon: "bi-arrow-up",
  },
  importCurl: {
    title: "Import cURL command(s)",
    icon: "bi-link",
  },
  importHar: {
    title: "Import HTTP archive (HAR)",
    icon: "bi-archive",
  },
  importOpenApi: {
    title: "Import OpenAPI definition",
    icon: "bi-cloud-upload",
  },
};

export default defineComponent({
  name: "ImportStubs",
  components: { ImportOpenApi, UploadStubs, ImportCurl, ImportHar },
  setup() {
    const router = useRouter();
    const route = useRoute();

    // Data
    const selectedTab = ref(route.query.tab || tabs.uploadStubs);

    // Methods
    const changeTab = async (tab: string) => {
      selectedTab.value = tab;
      await router.push({ name: "ImportStubs", query: { tab } });
    };

    return { tabs, tabDetails, selectedTab, changeTab };
  },
});
</script>

<style>
.tab-button img {
  width: 20px;
}
</style>
