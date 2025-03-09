<template>
  <div class="card">
    <div class="card-body">
      <div class="row">
        <div class="col-md-12" v-if="bodyType">
          <button class="btn btn-sm btn-primary me-2" @click="download">
            {{ $translate('general.download') }}
          </button>
          <button
            class="btn btn-sm me-2"
            :class="{
              'btn-outline-primary': showRenderedBody,
              'btn-outline-secondary': !showRenderedBody,
            }"
            @click="viewRenderedBody"
          >
            {{ bodyType }}
          </button>
          <button
            class="btn btn-sm me-2"
            :class="{
              'btn-outline-primary': !showRenderedBody,
              'btn-outline-secondary': showRenderedBody,
            }"
            @click="viewRawBody"
          >
            {{ $translate('request.raw') }}
          </button>
        </div>
      </div>
      <div class="row mt-3" :class="{ 'show-more': showMore }">
        <div class="col-md-12" v-if="showRenderedBody">
          <code-highlight :language="language" :code="renderedBody" />
        </div>
        <div class="col-md-12" v-if="!showRenderedBody">
          <code-highlight :code="body" />
        </div>
        <div v-if="showMore" @click="showMoreClick" class="show-more-button">
          <p class="text-center">
            <i class="bi bi-arrow-down-circle">&nbsp;</i>
          </p>
        </div>
      </div>
      <div class="row mt-3">
        <div class="col-md-12">
          <i
            class="bi bi-clipboard copy"
            :title="$translate('request.copyRequestBody')"
            @click="copy"
            >&nbsp;</i
          >
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, type PropType, ref } from 'vue'
import xmlFormatter from 'xml-formatter'
import { formFormat } from '@/utils/form'
import { copyTextToClipboard } from '@/utils/clipboard'
import { success } from '@/utils/toast'
import { countNewlineCharacters, fromBase64 } from '@/utils/text'
import { requestBodyLineLimit } from '@/constants'
import mime from 'mime-types'
import { downloadBlob } from '@/utils/download'
import type { RequestResponseBodyRenderModel } from '@/domain/request/request-response-body-render-model'
import { translate } from '@/utils/translate'

const bodyTypes = {
  xml: translate('request.xml'),
  json: translate('request.json'),
  form: translate('request.form'),
}

const props = defineProps({
  renderModel: {
    type: Object as PropType<RequestResponseBodyRenderModel>,
    required: true,
  },
})

// Data
const showRenderedBody = ref(false)
const showMoreClicked = ref(false)

// Computed
const body = computed<string>(() => {
  return props.renderModel.base64DecodeNotBinary
    ? fromBase64(props.renderModel.body) || ''
    : props.renderModel.body
})
const contentType = computed<string>(() => {
  const headers = props.renderModel.headers
  const contentTypeHeaderKey = Object.keys(headers).find((k) => k.toLowerCase() === 'content-type')
  if (!contentTypeHeaderKey) {
    return ''
  }

  return headers[contentTypeHeaderKey].toLowerCase().split(';')[0]
})
const bodyType = computed(() => {
  switch (contentType.value) {
    case 'text/xml':
    case 'application/xml':
    case 'application/soap+xml':
      return bodyTypes.xml
    case 'application/json':
      return bodyTypes.json
    case 'application/x-www-form-urlencoded':
      return bodyTypes.form
    default:
      return ''
  }
})
const renderedBody = computed(() => {
  if (bodyType.value === bodyTypes.xml) {
    return xmlFormatter(body.value)
  } else if (bodyType.value === bodyTypes.json) {
    try {
      const json = JSON.parse(body.value)
      return JSON.stringify(json, null, 2)
    } catch {
      return ''
    }
  } else if (bodyType.value === bodyTypes.form) {
    return formFormat(body.value)
  }

  return ''
})
const language = computed(() => {
  switch (bodyType.value) {
    case bodyTypes.json:
      return 'json'
    case bodyTypes.xml:
      return 'xml'
    default:
      return ''
  }
})
const showMoreButtonEnabled = computed(() => {
  const newlineCount = countNewlineCharacters(
    showRenderedBody.value ? renderedBody.value : body.value,
  )
  return newlineCount >= requestBodyLineLimit
})
const showMore = computed(() => {
  return showMoreButtonEnabled.value && !showMoreClicked.value
})

// Methods
const viewRenderedBody = () => {
  showRenderedBody.value = true
}
const viewRawBody = () => (showRenderedBody.value = false)
const copy = () => {
  const valueToCopy = showRenderedBody.value ? renderedBody.value : body.value
  copyTextToClipboard(valueToCopy).then(() =>
    success(translate('request.requestBodyCopiedToClipboard')),
  )
}
const showMoreClick = () => {
  showMoreClicked.value = true
}
const download = () => {
  const extension = mime.extension(contentType.value) ?? 'bin'
  downloadBlob(`file.${extension}`, body.value)
}

// Lifecycle
onMounted(() => {
  showRenderedBody.value = !!bodyType.value
})
</script>

<style scoped lang="scss">
@import '@/style/bootstrap';

.copy {
  font-size: 2em;
  cursor: pointer;
}

.show-more {
  height: 1200px;
  overflow: hidden;
  position: relative;
}

.show-more .show-more-button {
  width: 100%;
  height: 80px;
  position: absolute;
  bottom: 0;
  left: 0;
  cursor: pointer;
}

.show-more .show-more-button i {
  font-size: 3em;
  text-align: center;
}

.light-theme .show-more .show-more-button {
  background-image: linear-gradient(rgba(255, 0, 0, 0), #fff);
}

.dark-theme .show-more .show-more-button {
  background-image: linear-gradient(rgba(255, 0, 0, 0), $gray-900);
}
</style>
