<template>
  <slot name="modalbutton"></slot>
  <div class="modal" tabindex="-1" ref="modal">
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
        <div class="modal-body">
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
import { onMounted, ref } from "vue";

export default {
  name: "Modal",
  props: {
    title: {
      type: String,
      required: true,
    },
    bodyText: {
      type: String,
      required: true,
    },
    yesText: {
      type: String,
      default: "Yes",
    },
    noText: {
      type: String,
      default: "No",
    },
    yesClickFunction: {
      type: Function,
    },
    noClickFunction: {
      type: Function,
    },
  },
  setup(props, { slots }) {
    // Template refs
    const modal = ref(null);

    // Functions
    const toggleModal = () => {
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
      const button = slots.modalbutton()[0].el;
      button.addEventListener("click", () => {
        toggleModal();
      });
    });

    return { onYesClick, onNoClick, modal };
  },
};
</script>

<style scoped></style>
