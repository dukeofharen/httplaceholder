<template>
  <div class="modal fade" tabindex="-1" ref="modal" @keyup.enter="onYesClick">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">{{ title }}</h5>
          <button
            type="button"
            class="btn-close"
            data-bs-dismiss="modal"
            aria-label="Close"
          ></button>
        </div>
        <div class="modal-body" v-if="bodyText">
          <p>{{ bodyText }}</p>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" @click="onNoClick">
            {{ noText }}
          </button>
          <button type="button" class="btn btn-primary" @click="onYesClick">
            {{ yesText }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { Modal } from "bootstrap";
import { onMounted, ref, watch } from "vue";
import { defineComponent } from "vue";

export default defineComponent({
  name: "Modal",
  props: {
    title: {
      type: String,
      required: true,
    },
    bodyText: {
      type: String,
    },
    yesText: {
      type: String,
      default: "Yes",
    },
    noText: {
      type: String,
      default: "No",
    },
    showModal: {
      type: Boolean,
      default: false,
    },
    yesClickFunction: {
      type: Function,
    },
    noClickFunction: {
      type: Function,
    },
  },
  setup(props, { emit }) {
    // Template refs
    const modal = ref(null);

    // Functions
    const showModal = () => {
      const currentModal = Modal.getOrCreateInstance(modal.value);
      currentModal.show();
    };
    const hideModal = () => {
      const currentModal = Modal.getOrCreateInstance(modal.value);
      currentModal.hide();
    };

    // Methods
    const onYesClick = () => {
      if (props.yesClickFunction) {
        props.yesClickFunction();
      }

      hideModal();
    };
    const onNoClick = () => {
      if (props.noClickFunction) {
        props.noClickFunction();
      }

      hideModal();
    };

    // Lifecycle
    onMounted(() => {
      if (props.showModal) {
        showModal();
      } else {
        hideModal();
      }

      modal.value.addEventListener("hidden.bs.modal", () => {
        emit("close");
      });
    });

    // Watch
    watch(props, (newProps) => {
      if (newProps.showModal) {
        showModal();
      } else {
        hideModal();
      }
    });

    return { onYesClick, onNoClick, modal };
  },
});
</script>

<style scoped></style>
