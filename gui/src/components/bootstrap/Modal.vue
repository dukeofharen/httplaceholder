<template>
  <div class="modal fade" tabindex="-1" ref="modal" @keyup.enter="onYesClick">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header d-flex justify-content-between">
          <h5 class="modal-title">{{ title }}</h5>
          <button
            type="button"
            class="btn close-button"
            data-bs-dismiss="modal"
            :aria-label="$translate('general.close')"
          >
            <i class="bi bi-x"></i>
          </button>
        </div>
        <div class="modal-body" v-if="bodyText">
          <p>{{ bodyText }}</p>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-secondary no-button" @click="onNoClick">
            {{ getNoText() }}
          </button>
          <button type="button" class="btn btn-primary yes-button" @click="onYesClick">
            {{ getYesText() }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { getOrCreateInstance } from '@/utils/bootstrap'
import { translate } from '@/utils/translate'
import { onMounted, ref, watch } from 'vue'

const props = defineProps({
  title: {
    type: String,
    required: true,
  },
  bodyText: String,
  yesText: String,
  noText: String,
  showModal: {
    type: Boolean,
    default: false,
  },
  yesClickFunction: Function,
  noClickFunction: Function,
})
const emit = defineEmits(['close'])

// Data
const modal = ref<HTMLElement>()

// Functions
const showModal = () => {
  if (modal.value) {
    const currentModal = getOrCreateInstance(modal.value)
    currentModal.show()
  }
}

const hideModal = () => {
  if (modal.value) {
    const currentModal = getOrCreateInstance(modal.value)
    currentModal.hide()
  }
}

const onYesClick = () => {
  if (props.yesClickFunction) {
    props.yesClickFunction()
  }

  hideModal()
}

const onNoClick = () => {
  if (props.noClickFunction) {
    props.noClickFunction()
  }

  hideModal()
}

function getYesText() {
  return props.yesText ?? translate('general.yes')
}

function getNoText() {
  return props.noText ?? translate('general.no')
}

// Lifecycle
onMounted(() => {
  if (props.showModal) {
    showModal()
  } else {
    hideModal()
  }

  if (modal.value) {
    modal.value.addEventListener('hidden.bs.modal', () => {
      emit('close')
    })
  }
})

// Watch
watch(props, (newProps) => {
  if (newProps.showModal) {
    showModal()
  } else {
    hideModal()
  }
})
</script>

<style scoped lang="scss">
.close-button {
  margin: 0;
  padding: 0;

  .bi {
    font-size: 30px;
  }
}

.modal-title {
  max-width: 100%;
  text-wrap: nowrap;
  text-overflow: ellipsis;
  overflow: hidden;
}
</style>
