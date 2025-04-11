<template>
  <div class="row mt-3">
    <div class="col-md-12">
      <button
        v-for="button of formHelperButtons"
        :key="button.category"
        class="form-helper-button btn btn-outline-primary me-2 mt-2 mt-md-0"
        @click="openFormHelperList(button.category)"
      >
        {{ button.title }}
      </button>
    </div>
  </div>
  <Vue3SlideUpDown v-model="showFormHelperItems" :duration="300">
    <div class="row mt-3">
      <div class="col-md-12">
        <div class="mb-3">
          <button class="btn btn-danger btn-mobile full-width" @click="closeFormHelperAndList">
            {{ $translate('stubForm.closeList') }}
          </button>
        </div>
        <div class="input-group mb-3">
          <input
            type="text"
            class="form-control"
            :placeholder="$translate('stubForm.filterPlaceholder')"
            v-model="formHelperFilter"
            ref="formHelperFilterInput"
          />
        </div>
        <div class="list-group stub-form-helpers">
          <template v-for="(item, index) in filteredStubFormHelpers" :key="index">
            <h2 v-if="item.isHeading" class="list-group-item">
              {{ item.title }}
            </h2>
            <button
              v-else
              class="list-group-item list-group-item-action"
              @click="onFormHelperItemClick(item)"
            >
              <label class="fw-bold">{{ item.title }}</label>
              <span class="subtitle">{{ item.subTitle }}</span>
            </button>
          </template>
        </div>
      </div>
    </div>
  </Vue3SlideUpDown>
  <div v-if="currentSelectedFormHelper" class="row mt-3">
    <div class="col-md-12">
      <div class="card">
        <div class="card-body">
          <RenderedFormHelper />
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { computed, defineComponent, onMounted, onUnmounted, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { escapePressed } from '@/utils/event'
import { useStubFormStore } from '@/store/stubForm'
import {
  type StubFormHelper,
  StubFormHelperCategory,
  stubFormHelpers,
} from '@/domain/stubForm/stub-form-helpers'
import { translate } from '@/utils/translate'

export default defineComponent({
  name: 'FormHelperSelector',
  setup() {
    const stubFormStore = useStubFormStore()
    const route = useRoute()

    // Refs
    const formHelperFilterInput = ref<HTMLElement>()

    // Data
    const showFormHelperItems = ref(false)
    const formHelperButtons = [
      {
        title: translate('stubForm.addExample'),
        category: StubFormHelperCategory.Examples,
      },
      {
        title: translate('stubForm.addGeneralStubInfo'),
        category: StubFormHelperCategory.GeneralInfo,
      },
      {
        title: translate('stubForm.addRequestCondition'),
        category: StubFormHelperCategory.RequestCondition,
      },
      {
        title: translate('stubForm.addResponseWriter'),
        category: StubFormHelperCategory.ResponseDefinition,
      },
    ]
    const selectedFormHelperCategory = ref<StubFormHelperCategory>(StubFormHelperCategory.None)

    // Methods
    const onFormHelperItemClick = (item: StubFormHelper) => {
      if (item.defaultValueMutation) {
        item.defaultValueMutation(stubFormStore)
        stubFormStore.closeFormHelper()
      } else if (item.formHelperToOpen) {
        stubFormStore.openFormHelper(item.formHelperToOpen)
      }

      showFormHelperItems.value = false
      formHelperFilter.value = ''
    }
    const openFormHelperList = (category: StubFormHelperCategory) => {
      if (selectedFormHelperCategory.value === category) {
        closeFormHelperAndList()
        return
      }

      showFormHelperItems.value = true
      selectedFormHelperCategory.value = category
      const formHelpers = stubFormHelpers.filter((h) => h.stubFormHelperCategory === category)
      if (formHelpers.length === 1) {
        onFormHelperItemClick(formHelpers[0])
      } else {
        setTimeout(() => {
          if (formHelperFilterInput.value) {
            formHelperFilterInput.value.focus()
          }
        }, 10)
      }
    }
    const closeFormHelperAndList = () => {
      formHelperFilter.value = ''
      stubFormStore.closeFormHelper()
      showFormHelperItems.value = false
      selectedFormHelperCategory.value = StubFormHelperCategory.None
    }

    // Computed
    const currentSelectedFormHelper = computed(() => stubFormStore.getCurrentSelectedFormHelper)
    const filteredStubFormHelpers = computed(() => {
      let result = stubFormHelpers
      if (selectedFormHelperCategory.value) {
        result = result.filter((r) => r.stubFormHelperCategory === selectedFormHelperCategory.value)
      }

      if (!formHelperFilter.value) {
        return result
      }

      return result.filter((h) => {
        return !h.isHeading && h.title.toLowerCase().includes(formHelperFilter.value.toLowerCase())
      })
    })
    const formHelperFilter = computed({
      get: () => stubFormStore.getFormHelperSelectorFilter,
      set: (value) => stubFormStore.setFormHelperSelectorFilter(value),
    })

    // Watch
    watch(currentSelectedFormHelper, (formHelper) => {
      if (!formHelper) {
        showFormHelperItems.value = false
      }
    })
    watch(
      () => route.params,
      () => closeFormHelperAndList(),
    )
    watch(showFormHelperItems, (newValue) => {
      if (!newValue) {
        selectedFormHelperCategory.value = StubFormHelperCategory.None
      }
    })

    // Lifecycle
    const escapeListener = (e: KeyboardEvent) => {
      if (escapePressed(e)) {
        e.preventDefault()
        closeFormHelperAndList()
      }
    }
    onMounted(() => document.addEventListener('keydown', escapeListener))
    onUnmounted(() => document.removeEventListener('keydown', escapeListener))

    return {
      currentSelectedFormHelper,
      showFormHelperItems,
      filteredStubFormHelpers,
      onFormHelperItemClick,
      formHelperFilter,
      formHelperFilterInput,
      openFormHelperList,
      closeFormHelperAndList,
      formHelperButtons,
      selectedFormHelperCategory,
    }
  },
})
</script>

<style scoped lang="scss">
@import '@/style/bootstrap';

label {
  display: block;
  cursor: pointer;
}

.subtitle {
  font-size: 0.9em;
}

.stub-form-helpers {
  h2 {
    margin: 0;
  }
}

@include media-breakpoint-down(md) {
  .form-helper-button {
    width: 100%;
  }
}
</style>
