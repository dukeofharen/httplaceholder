<template>
  <div class="mb-2 col-md-6">
    {{ $translate('importOpenApi.intro') }}
  </div>
  <div v-if="!stubsPreviewOpened">
    <div class="mb-2">
      <button class="btn btn-outline-primary btn-sm" @click="insertExample">
        {{ $translate('importStubs.insertExample') }}
      </button>
    </div>
    <div class="mb-2">
      <upload-button :button-text="$translate('importStubs.uploadFile')" @uploaded="onUploaded" />
    </div>
    <div class="mb-2">
      <input
        type="text"
        class="form-control"
        :placeholder="$translate('importStubs.tenantPlaceholder')"
        v-model="tenant"
      />
    </div>
    <div class="mb-2">
      <input
        type="text"
        class="form-control"
        :placeholder="$translate('importStubs.stubIdPrefixPlaceholder')"
        v-model="stubIdPrefix"
      />
    </div>
    <div class="mb-2">
      <textarea class="form-control" v-model="input"></textarea>
    </div>
    <div class="mb-2">
      <button class="btn btn-success" @click="importOpenApi" :disabled="!importButtonEnabled">
        {{ $translate('importOpenApi.importOpenApiDefinition') }}
      </button>
    </div>
  </div>
  <div v-else>
    <div class="mb-2">{{ $translate('importStubs.stubsWillBeAdded') }}</div>
    <div class="mb-2">
      <button class="btn btn-success me-2" @click="saveStubs">
        {{ $translate('importStubs.saveStubs') }}
      </button>
      <button class="btn btn-success me-2" @click="editBeforeSaving">
        {{ $translate('importStubs.editStubsBeforeSaving') }}
      </button>
      <button class="btn btn-danger me-2" @click="reset">
        {{ $translate('general.reset') }}
      </button>
    </div>
    <div class="mb-2">
      <code-highlight language="yaml" :code="stubsYaml" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { computed, onMounted, onUnmounted, ref } from 'vue'
import yaml from 'js-yaml'
import { handleHttpError } from '@/utils/error'
import { setIntermediateStub } from '@/utils/session'
import { shouldSave } from '@/utils/event'
import { success } from '@/utils/toast'
import { type ImportInputModel, useImportStore } from '@/store/import'
import type { FileUploadedModel } from '@/domain/file-uploaded-model'
import { exampleOpenApiInput } from '@/strings/exmaples'
import { translate } from '@/utils/translate'

const importStore = useImportStore()
const router = useRouter()

// Data
const input = ref('')
const stubsYaml = ref('')
const tenant = ref('')
const stubIdPrefix = ref('')

// Computed
const importButtonEnabled = computed(() => !!input.value)
const stubsPreviewOpened = computed(() => !!stubsYaml.value)

// Functions
const buildInputModel = (doNotCreateStub: boolean): ImportInputModel => {
  return {
    doNotCreateStub: doNotCreateStub,
    tenant: tenant.value,
    input: input.value,
    stubIdPrefix: stubIdPrefix.value,
  }
}

// Methods
const insertExample = () => {
  input.value = exampleOpenApiInput
}
const importOpenApi = async () => {
  try {
    const importInput = buildInputModel(true)
    const result = await importStore.importOpenApi(importInput)

    const filteredResult = result.map((r) => r.stub)
    stubsYaml.value = yaml.dump(filteredResult)
  } catch (e) {
    handleHttpError(e)
  }
}
const onUploaded = (file: FileUploadedModel) => {
  input.value = file.result
}
const saveStubs = async () => {
  try {
    const importInput = buildInputModel(false)
    await importStore.importOpenApi(importInput)
    success(translate('importStubs.stubsAddedSuccessfully'))
    await router.push({ name: 'Stubs' })
  } catch (e) {
    handleHttpError(e)
  }
}
const editBeforeSaving = () => {
  setIntermediateStub(stubsYaml.value)
  router.push({ name: 'StubForm' })
}
const reset = () => {
  input.value = ''
  stubsYaml.value = ''
  tenant.value = ''
}

// Lifecycle
const handleSave = async (e: KeyboardEvent) => {
  if (shouldSave(e)) {
    e.preventDefault()
    if (!stubsYaml.value) {
      await importOpenApi()
    } else {
      await saveStubs()
    }
  }
}
const keydownEventListener = async (e: KeyboardEvent) => await handleSave(e)
onMounted(() => document.addEventListener('keydown', keydownEventListener))
onUnmounted(() => document.removeEventListener('keydown', keydownEventListener))
</script>

<style scoped>
textarea {
  min-height: 300px;
}
</style>
