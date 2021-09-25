<template>
  <div class="accordion-item">
    <h2 class="accordion-header" :id="headingId">
      <button
        class="accordion-button collapsed"
        type="button"
        data-bs-toggle="collapse"
        :data-bs-target="'#' + contentId"
        aria-expanded="false"
        :aria-controls="contentId"
        @click="showDetails"
      >
        <span>
          {{ overviewStub.stub.id }}
        </span>
      </button>
    </h2>
    <div
      :id="contentId"
      class="accordion-collapse collapse"
      :aria-labelledby="headingId"
      :data-bs-parent="'#' + accordionId"
    >
      <div v-if="fullStub" class="accordion-body">
        <pre>{{ stubYaml }}</pre>
      </div>
    </div>
  </div>
</template>

<script>
import { computed, ref } from "vue";
import { useStore } from "vuex";
import yaml from "js-yaml";

export default {
  name: "Stub",
  props: {
    overviewStub: {
      type: Object,
      required: true,
    },
    accordionId: {
      type: String,
      required: true,
    },
  },
  setup(props) {
    const store = useStore();

    // Functions
    const getStubId = () => props.overviewStub.stub.id;

    // Data
    const fullStub = ref(null);

    // Computed
    const headingId = computed(() => `stubheading-${getStubId()}`);
    const contentId = computed(() => `stubcontent-${getStubId()}`);
    const stubYaml = computed(() => {
      if (!fullStub.value) {
        return "";
      }

      return yaml.dump(fullStub.value.stub);
    });

    // Methods
    const showDetails = async () => {
      if (!fullStub.value) {
        fullStub.value = await store.dispatch("stubs/getStub", getStubId());
      }
    };

    return { headingId, contentId, showDetails, fullStub, stubYaml };
  },
};
</script>

<style scoped></style>
