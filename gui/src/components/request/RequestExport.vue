<template>
  <div class="mt-3 mb-3">
    <select class="form-select" v-model="exportType">
      <option :value="RequestExportType.NotSet">
        Select an export format...
      </option>
      <option :value="RequestExportType.Curl">cURL</option>
      <option v-if="request.hasResponse" :value="RequestExportType.Har">
        HTTP Archive (HAR)
      </option>
    </select>
    <div v-if="showExportResult" class="code-copy-wrapper mt-2">
      <div class="icon-wrapper">
        <i
          class="bi bi-clipboard copy"
          title="Copy command to clipboard"
          @click="copy"
        ></i>
      </div>
      <code-highlight
        class="m-0"
        :language="language"
        :code="exportResult"
      ></code-highlight>
    </div>
  </div>
</template>

<script lang="ts">
import { computed, defineComponent, PropType, ref, watch } from "vue";
import { RequestExportType } from "@/domain/request/enums/request-export-type";
import { useExportStore } from "@/store/export";
import { handleHttpError } from "@/utils/error";
import { copyTextToClipboard } from "@/utils/clipboard";
import { RequestResultModel } from "@/domain/request/request-result-model";

export default defineComponent({
  props: {
    request: {
      type: Object as PropType<RequestResultModel>,
      required: true,
    },
  },
  setup(props) {
    const exportStore = useExportStore();

    // Data
    const exportType = ref(RequestExportType.NotSet);
    const exportResult = ref("");

    // Computed
    const language = computed(() => {
      switch (exportType.value) {
        case RequestExportType.Curl:
          return "bash";
        default:
          return "plaintext";
      }
    });
    const showExportResult = computed(() => {
      return (
        exportType.value !== RequestExportType.NotSet && !!exportResult.value
      );
    });

    // Methods
    const exportRequest = async () => {
      try {
        const result = await exportStore.exportRequest(
          props.request.correlationId,
          exportType.value,
        );
        exportResult.value = result.result;
      } catch (e) {
        handleHttpError(e);
      }
    };
    const copy = async () => {
      if (exportResult.value) {
        await copyTextToClipboard(exportResult.value);
      }
    };

    // Watches
    watch(exportType, async (newType) => {
      if (newType !== RequestExportType.NotSet) {
        exportResult.value = "";
        await exportRequest();
      }
    });

    return {
      RequestExportType,
      exportType,
      language,
      exportResult,
      showExportResult,
      copy,
    };
  },
});
</script>

<style scoped lang="scss">
.code-copy-wrapper {
  display: flex;
  flex-direction: row;
  align-items: center;
  gap: 10px;

  .icon-wrapper {
    display: flex;
    flex-direction: row;
    align-items: center;
    .copy {
      cursor: pointer;
    }
  }
}
</style>
