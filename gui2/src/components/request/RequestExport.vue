<script setup lang="ts">
import { computed, type PropType, ref, watch } from 'vue'
import type { RequestResultModel } from '@/domain/request/request-result-model.ts'
import SelectInput from '@/components/html-elements/SelectInput.vue'
import type { SelectListItem } from '@/domain/ui/select-list-item.ts'
import { translate } from '@/utils/translate.ts'
import { RequestExportType } from '@/domain/request/enums/request-export-type.ts'
import { useExportStore } from '@/stores/export.ts'
import { handleHttpError } from '@/utils/error.ts'
import { copyTextToClipboard } from '@/utils/clipboard.ts'
import { downloadBlob } from '@/utils/download.ts'
import ButtonComponent from '@/components/html-elements/ButtonComponent.vue'
import { ClipboardIcon } from '@heroicons/vue/24/solid'
import { success } from '@/utils/toast.ts'
import CodeHighlight from '@/components/highlight/CodeHighlight.vue'

const props = defineProps({
  request: {
    type: Object as PropType<RequestResultModel>,
    required: true,
  },
})
const exportStore = useExportStore()

// Data
const exportType = ref(RequestExportType.NotSet)
const exportResult = ref('')

// Computed
const selectListItems = computed<SelectListItem[]>(() => {
  const result = [
    {
      label: translate('request.selectExportFormat'),
      value: RequestExportType.NotSet,
    },
    {
      label: translate('request.curl'),
      value: RequestExportType.Curl,
    },
  ]
  if (props.request.hasResponse) {
    result.push({
      label: translate('request.har'),
      value: RequestExportType.Har,
    })
  }

  return result
})
const language = computed(() => {
  switch (exportType.value) {
    case RequestExportType.Curl:
      return 'bash'
    default:
      return 'plaintext'
  }
})
const showExportResult = computed(() => {
  return exportType.value !== RequestExportType.NotSet && !!exportResult.value
})
const showExportResultText = computed(() => {
  switch (exportType.value) {
    case RequestExportType.Curl:
      return true
    default:
      return false
  }
})
const exportFilename = computed(() => {
  switch (exportType.value) {
    case RequestExportType.Har:
      return `har-${props.request.correlationId}.json`
    default:
      return 'file.bin'
  }
})

// Methods
const exportRequest = async () => {
  try {
    const result = await exportStore.exportRequest(props.request.correlationId, exportType.value)
    exportResult.value = result.result
  } catch (e) {
    handleHttpError(e)
  }
}
const copy = async () => {
  if (exportResult.value) {
    await copyTextToClipboard(exportResult.value)
    success(translate('request.requestCopiedToClipboard'))
  }
}
const download = async () => {
  downloadBlob(exportFilename.value, exportResult.value)
}

// Watches
watch(exportType, async (newType) => {
  if (newType !== RequestExportType.NotSet) {
    exportResult.value = ''
    await exportRequest()
  }
})
</script>

<template>
  <div class="flex flex-col gap-1">
    <SelectInput id="exportType" :items="selectListItems" v-model="exportType" />
    <div v-if="showExportResult" class="mt-2">
      <div v-if="showExportResultText" class="flex flex-row items-center gap-2 overflow-x-scroll">
        <div>
          <ClipboardIcon class="size-6 cursor-pointer" @click="copy" />
        </div>
        <CodeHighlight class="" :code="exportResult" :language="language" />
      </div>
      <template v-else>
        <ButtonComponent
          type="success"
          @click="download"
          :title="$translate('request.downloadExportedRequest')"
          :dense="true"
          >{{ $translate('general.download') }}
        </ButtonComponent>
      </template>
    </div>
  </div>
</template>
