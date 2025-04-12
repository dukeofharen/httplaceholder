<template>
  <div class="card">
    <div class="card-body">
      <div class="row">
        <div class="col-md-12">
          <button class="btn btn-sm btn-primary" @click="download">
            {{ $translate('general.download') }}
          </button>
          <div v-if="bodyType != bodyTypes.other" class="mt-2">
            <div v-if="bodyType === bodyTypes.image">
              <img :src="dataUrl" />
            </div>
            <div v-if="bodyType === bodyTypes.pdf">
              <VuePdfEmbed :source="dataUrl" class="pdf-viewer" />
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { downloadBlob } from '@/utils/download'
import { base64ToBlob } from '@/utils/text'
import { imageMimeTypes, pdfMimeType } from '@/constants'
import mime from 'mime-types'
import type { RequestResponseBodyRenderModel } from '@/domain/request/request-response-body-render-model'

const bodyTypes = {
  image: 'image',
  pdf: 'pdf',
  other: 'other',
}

export type BinaryBodyProps = {
  renderModel: RequestResponseBodyRenderModel
}
const props = defineProps<BinaryBodyProps>()

// Computed
const body = computed(() => {
  return props.renderModel.body
})

const contentType = computed(() => {
  const headers = props.renderModel.headers
  const contentTypeHeaderKey = Object.keys(headers).find((k) => k.toLowerCase() === 'content-type')
  if (!contentTypeHeaderKey) {
    return ''
  }

  return headers[contentTypeHeaderKey].toLowerCase().split(';')[0]
})

const bodyType = computed(() => {
  const type = contentType.value
  if (!type) {
    return bodyTypes.other
  }

  if (imageMimeTypes.find((m) => type.includes(m))) {
    return bodyTypes.image
  }

  if (type.includes(pdfMimeType)) {
    return bodyTypes.pdf
  }

  return bodyTypes.other
})

const dataUrl = computed(() => {
  return `data:${contentType.value};base64,${body.value}`
})

// Methods
const download = () => {
  const extension = mime.extension(contentType.value) ?? 'bin'
  downloadBlob(`file.${extension}`, base64ToBlob(body.value))
}
</script>

<style scoped lang="scss">
@import '@/style/bootstrap';

img {
  max-width: 100%;
}

.pdf-viewer {
  min-height: 500px;
  max-height: 1000px;
  overflow-x: scroll;

  @include media-breakpoint-down(lg) {
    width: 100%;
  }
  @include media-breakpoint-up(xl) {
    width: 50%;
  }
}
</style>
