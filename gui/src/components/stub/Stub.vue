<template>
  <accordion-item @buttonClicked="showDetails" :opened="accordionOpened">
    <template v-slot:button-text>
      <span :class="{ disabled: !isEnabled }">
        {{ id }}
      </span>
      <span v-if="!isEnabled" class="disabled">&nbsp;({{ $translate('stubs.disabled') }})</span>
      <span v-if="overviewStub.metadata.readOnly" :title="$translate('stubs.stubIsReadonly')"
        >&nbsp;<i class="bi-eye"></i
      ></span>
    </template>
    <template v-slot:accordion-body>
      <div v-if="fullStub">
        <div class="row mb-3">
          <div class="col-md-12">
            <router-link
              class="btn btn-success btn-sm me-2 btn-mobile"
              :title="$translate('stubs.viewAllRequests')"
              :to="{
                name: 'Requests',
                query: { filter: id },
              }"
              >{{ $translate('general.requests') }}
            </router-link>
            <button
              class="btn btn-success btn-sm me-2 btn-mobile"
              :title="$translate('stubs.duplicateThisStub')"
              @click="duplicate"
            >
              {{ $translate('stubs.duplicate') }}
            </button>
            <router-link
              v-if="!isReadOnly"
              class="btn btn-success btn-sm me-2 btn-mobile"
              :title="$translate('stubs.updateStub')"
              :to="{
                name: 'StubForm',
                params: { stubId: id },
              }"
              >{{ $translate('general.update') }}
            </router-link>
            <button
              v-if="!isReadOnly"
              class="btn btn-success btn-sm me-2 btn-mobile"
              :title="isEnabled ? $translate('stubs.disableStub') : $translate('stubs.enableStub')"
              @click="enableOrDisable"
            >
              {{ isEnabled ? $translate('stubs.disable') : $translate('stubs.enable') }}
            </button>
            <button class="btn btn-success btn-sm me-2 btn-mobile" @click="downloadStub">
              {{ $translate('general.download') }}
            </button>
            <router-link
              v-if="hasScenario"
              class="btn btn-success btn-sm me-2 btn-mobile"
              :to="{ name: 'ScenarioForm', params: { scenario: scenario } }"
              >{{ $translate('stubs.setScenario') }}
            </router-link>
            <button
              v-if="!isReadOnly"
              class="btn btn-danger btn-sm me-2 btn-mobile"
              :title="$translate('stubs.deleteStub')"
              @click="showDeleteModal = true"
            >
              {{ $translate('general.delete') }}
            </button>
            <modal
              v-if="!isReadOnly"
              :title="$vsprintf($translate('stubs.deleteStubWithId'), [id])"
              :bodyText="$translate('stubs.stubsCantBeRecovered')"
              :show-modal="showDeleteModal"
              @close="showDeleteModal = false"
              @yes-click="deleteStub"
            />
          </div>
        </div>
        <div v-if="overviewStub.metadata.filename" class="stub-location">
          {{ $translate('stubs.stubLocation') }}:
          {{ overviewStub.metadata.filename }}
        </div>
        <code-highlight language="yaml" :code="stubYaml" />
      </div>
    </template>
  </accordion-item>
</template>

<script lang="ts">
import { computed, type PropType, ref } from 'vue'
import yaml from 'js-yaml'
import { setIntermediateStub } from '@/utils/session'
import { useRouter } from 'vue-router'
import dayjs from 'dayjs'
import { handleHttpError } from '@/utils/error'
import { success } from '@/utils/toast'
import { useStubsStore } from '@/store/stubs'
import { defineComponent } from 'vue'
import type { FullStubOverviewModel } from '@/domain/stub/full-stub-overview-model'
import type { FullStubModel } from '@/domain/stub/full-stub-model'
import { downloadBlob } from '@/utils/download'
import { vsprintf } from 'sprintf-js'
import { translate } from '@/utils/translate'

export default defineComponent({
  name: 'Stub',
  props: {
    overviewStub: {
      type: Object as PropType<FullStubOverviewModel>,
      required: true,
    },
  },
  setup(props, { emit }) {
    const stubStore = useStubsStore()
    const router = useRouter()

    // Data
    const overviewStubValue = ref(props.overviewStub)
    const fullStub = ref<FullStubModel>()
    const showDeleteModal = ref(false)
    const accordionOpened = ref(false)

    // Computed
    const stubYaml = computed(() => {
      if (!fullStub.value) {
        return ''
      }

      return yaml.dump(fullStub.value.stub)
    })
    const scenario = computed(() => {
      if (!fullStub.value) {
        return null
      }

      return fullStub.value.stub.scenario
    })
    const hasScenario = computed(() => {
      return !!scenario.value
    })
    const isReadOnly = computed(() => (fullStub.value ? fullStub.value.metadata.readOnly : true))
    const isEnabled = computed(() => props.overviewStub.stub.enabled)
    const id = computed(() => overviewStubValue.value.stub.id)

    // Methods
    const showDetails = async () => {
      if (!fullStub.value) {
        try {
          fullStub.value = await stubStore.getStub(id.value)

          // Sadly, when doing this without the timeout, it does the slide down incorrect.
          setTimeout(() => (accordionOpened.value = true), 1)
        } catch (e) {
          handleHttpError(e)
        }
      } else {
        accordionOpened.value = !accordionOpened.value
      }
    }
    const duplicate = async () => {
      if (fullStub.value && fullStub.value.stub) {
        const stub = fullStub.value.stub
        stub.id = `${stub.id}_${dayjs().format('YYYY-MM-DD_HH-mm-ss')}`
        setIntermediateStub(yaml.dump(stub))
        await router.push({ name: 'StubForm' })
      }
    }
    const enableOrDisable = async () => {
      if (fullStub.value) {
        try {
          const enabled = await stubStore.flipEnabled(id.value)
          fullStub.value.stub.enabled = enabled
          overviewStubValue.value.stub.enabled = enabled
          let message
          if (enabled) {
            message = vsprintf(translate('stubs.stubEnabledSuccessfully'), [id.value])
          } else {
            message = vsprintf(translate('stubs.stubDisabledSuccessfully'), [id.value])
          }

          success(message)
        } catch (e) {
          handleHttpError(e)
        }
      }
    }
    const deleteStub = async () => {
      try {
        await stubStore.deleteStub(id.value)
        success(translate('stubs.stubDeletedSuccessfully'))
        showDeleteModal.value = false
        emit('deleted')
      } catch (e) {
        handleHttpError(e)
      }
    }
    const downloadStub = async () => {
      try {
        const fullStub = await stubStore.getStub(id.value)
        const stub = fullStub.stub
        const downloadString = `${translate('stubs.downloadStubsHeader')}\n${yaml.dump(stub)}`
        downloadBlob(`${stub.id}-stub.yml`, downloadString)
      } catch (e) {
        handleHttpError(e)
      }
    }

    return {
      showDetails,
      fullStub,
      stubYaml,
      duplicate,
      isReadOnly,
      enableOrDisable,
      overviewStubValue,
      deleteStub,
      showDeleteModal,
      id,
      accordionOpened,
      hasScenario,
      scenario,
      downloadStub,
      isEnabled,
    }
  },
})
</script>

<style scoped>
.disabled {
  color: #969696;
}

.stub-location {
  font-size: 12px;
  margin: 10px 0;
}
</style>
