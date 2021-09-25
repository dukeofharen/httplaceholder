<template>
  <accordion-item @opened="showDetails">
    <template v-slot:button-text>
      <span :class="{ disabled: !enabled }">
        {{ id }}
      </span>
    </template>
    <template v-slot:accordion-body>
      <div v-if="fullStub">
        <div class="row mb-3">
          <div class="col-md-12">
            <router-link
              class="btn btn-success btn-sm me-2"
              title="View all requests made for this stub"
              :to="{
                name: 'Requests',
                query: { filter: id },
              }"
              >Requests</router-link
            >
            <button
              class="btn btn-success btn-sm me-2"
              title="Duplicate this stub"
              @click="duplicate"
            >
              Duplicate
            </button>
            <router-link
              v-if="!isReadOnly"
              class="btn btn-success btn-sm me-2"
              title="Update this stub"
              :to="{
                name: 'StubForm',
                params: { stubId: id },
              }"
              >Update</router-link
            >
            <button
              v-if="!isReadOnly"
              class="btn btn-success btn-sm me-2"
              :title="enableDisableTitle"
              @click="enableOrDisable"
            >
              {{ enableDisableText }}
            </button>
            <button
              v-if="!isReadOnly"
              class="btn btn-danger btn-sm me-2"
              title="Delete the stub"
              @click="showDeleteModal = true"
            >
              Delete
            </button>
            <modal
              v-if="!isReadOnly"
              :title="deleteStubTitle"
              bodyText="The stub can't be recovered."
              :yes-click-function="deleteStub"
              :show-modal="showDeleteModal"
              @close="showDeleteModal = false"
            />
          </div>
        </div>
        <pre>{{ stubYaml }}</pre>
      </div>
    </template>
  </accordion-item>
</template>

<script>
import { computed, ref } from "vue";
import { useStore } from "vuex";
import yaml from "js-yaml";
import toastr from "toastr";
import { resources } from "@/constants/resources";
import AccordionItem from "@/components/bootstrap/AccordionItem";

export default {
  name: "Stub",
  components: { AccordionItem },
  props: {
    overviewStub: {
      type: Object,
      required: true,
    },
  },
  setup(props, { emit }) {
    const store = useStore();

    // Functions
    const getStubId = () => props.overviewStub.stub.id;
    const isEnabled = () => props.overviewStub.stub.enabled;

    // Data
    const overviewStubValue = ref(props.overviewStub);
    const fullStub = ref(null);
    const showDeleteModal = ref(false);

    // Computed
    const stubYaml = computed(() => {
      if (!fullStub.value) {
        return "";
      }

      return yaml.dump(fullStub.value.stub);
    });
    const isReadOnly = computed(() =>
      fullStub.value ? fullStub.value.metadata.readOnly : true
    );
    const enableDisableTitle = computed(
      () => `${isEnabled() ? "Disable" : "Enable"} stub`
    );
    const enableDisableText = computed(() =>
      isEnabled() ? "Disable" : "Enable"
    );
    const enabled = computed(() => isEnabled());
    const deleteStubTitle = computed(() => `Delete stub '${getStubId()}'?`);
    const id = computed(() => overviewStubValue.value.stub.id);

    // Methods
    const showDetails = async () => {
      if (!fullStub.value) {
        fullStub.value = await store.dispatch("stubs/getStub", getStubId());
      }
    };
    const duplicate = () => alert("TODO"); // TODO
    const enableOrDisable = async () => {
      const enabled = await store.dispatch("stubs/flipEnabled", getStubId());
      fullStub.value.stub.enabled = enabled;
      overviewStubValue.value.stub.enabled = enabled;
    };
    const deleteStub = async () => {
      await store.dispatch("stubs/deleteStub", getStubId());
      toastr.success(resources.stubDeletedSuccessfully);
      showDeleteModal.value = false;
      emit("deleted");
    };

    return {
      showDetails,
      fullStub,
      stubYaml,
      duplicate,
      isReadOnly,
      enableOrDisable,
      enableDisableTitle,
      enableDisableText,
      overviewStubValue,
      deleteStub,
      deleteStubTitle,
      showDeleteModal,
      id,
      enabled,
    };
  },
};
</script>

<style scoped>
.disabled {
  color: #969696;
}
</style>
