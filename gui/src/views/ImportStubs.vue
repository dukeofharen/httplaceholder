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
        @click="selectedTab = tab"
      >
        {{ tabNames[tab] }}
      </button>
    </div>
    <div class="col-md-12 mt-3" v-if="selectedTab === tabs.uploadStubs">
      <div class="mb-2">
        Press the button below to upload a YAML file with stubs.
      </div>
      <UploadStubs @uploaded="stubsUploaded" />
    </div>
  </div>
</template>

<script>
import UploadStubs from "@/components/stub/UploadStubs";
import { ref } from "vue";
import { useRouter } from "vue-router";

const tabs = {
  uploadStubs: "uploadStubs",
  importCurl: "importCurl",
};

const tabNames = {
  uploadStubs: "Upload stubs",
  importCurl: "Import cURL command(s)",
};

export default {
  name: "ImportStubs",
  components: { UploadStubs },
  setup() {
    const router = useRouter();

    // Data
    const selectedTab = ref(tabs.uploadStubs);

    // Methods
    const stubsUploaded = async () => await router.push({ name: "Stubs" });

    return { tabs, tabNames, selectedTab, stubsUploaded };
  },
};
</script>

<style scoped></style>
