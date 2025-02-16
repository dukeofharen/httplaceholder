<template>
  <div>
    <h1>{{ $translate('importStubs.importStubs') }}</h1>
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
        <i v-if="tabDetails[tab].icon" class="bi" :class="tabDetails[tab].icon" />
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
import UploadStubs from '@/components/import/UploadStubs.vue'
import ImportCurl from '@/components/import/ImportCurl.vue'
import ImportHar from '@/components/import/ImportHar.vue'
import ImportOpenApi from '@/components/import/ImportOpenApi.vue'
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { defineComponent } from 'vue'
import { translate } from '@/utils/translate'

const tabs = {
  uploadStubs: 'uploadStubs',
  importCurl: 'importCurl',
  importHar: 'importHar',
  importOpenApi: 'importOpenApi',
}

const tabDetails: any = {
  uploadStubs: {
    title: translate('importStubs.uploadStubs'),
    icon: 'bi-arrow-up',
  },
  importCurl: {
    title: translate('importStubs.importCurlCommands'),
    icon: 'bi-link',
  },
  importHar: {
    title: translate('importStubs.importHar'),
    icon: 'bi-archive',
  },
  importOpenApi: {
    title: translate('importStubs.importOpenApi'),
    icon: 'bi-cloud-upload',
  },
}

export default defineComponent({
  name: 'ImportStubs',
  components: { ImportOpenApi, UploadStubs, ImportCurl, ImportHar },
  setup() {
    const router = useRouter()
    const route = useRoute()

    // Data
    const selectedTab = ref(route.query.tab || tabs.uploadStubs)

    // Methods
    const changeTab = async (tab: string) => {
      selectedTab.value = tab
      await router.push({ name: 'ImportStubs', query: { tab } })
    }

    return { tabs, tabDetails, selectedTab, changeTab }
  },
})
</script>

<style>
.tab-button img {
  width: 20px;
}
</style>
