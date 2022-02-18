<template>
  <div class="row mt-3" v-if="!showFormHelperItems">
    <div class="col-md-12">
      <button class="btn btn-outline-primary" @click="openFormHelperList">
        Add request / response value
      </button>
    </div>
  </div>
  <div class="row mt-3" v-if="showFormHelperItems">
    <div class="col-md-12">
      <div class="mb-3">
        <button
          class="btn btn-danger btn-mobile full-width"
          @click="closeFormHelperAndList"
        >
          Close list
        </button>
      </div>
      <div class="input-group mb-3">
        <input
          type="text"
          class="form-control"
          placeholder="Filter form helpers (press 'Escape' to close)..."
          v-model="formHelperFilter"
          ref="formHelperFilterInput"
        />
      </div>
      <div class="list-group">
        <template v-for="(item, index) in filteredStubFormHelpers" :key="index">
          <div v-if="item.isMainItem" class="list-group-item fw-bold fs-3">
            {{ item.title }}
          </div>
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
  <div v-if="currentSelectedFormHelper" class="row mt-3">
    <div class="col-md-12">
      <div class="card">
        <div class="card-body">
          <HttpMethodSelector
            v-if="currentSelectedFormHelper === formHelperKeys.httpMethod"
          />
          <TenantSelector
            v-if="currentSelectedFormHelper === formHelperKeys.tenant"
          />
          <HttpStatusCodeSelector
            v-if="currentSelectedFormHelper === formHelperKeys.statusCode"
          />
          <ResponseBodyHelper
            v-if="currentSelectedFormHelper === formHelperKeys.responseBody"
          />
          <ResponseBodyHelper
            v-if="
              currentSelectedFormHelper === formHelperKeys.responseBodyPlainText
            "
            :preset-response-body-type="responseBodyTypes.text"
          />
          <ResponseBodyHelper
            v-if="currentSelectedFormHelper === formHelperKeys.responseBodyJson"
            :preset-response-body-type="responseBodyTypes.json"
          />
          <ResponseBodyHelper
            v-if="currentSelectedFormHelper === formHelperKeys.responseBodyXml"
            :preset-response-body-type="responseBodyTypes.xml"
          />
          <ResponseBodyHelper
            v-if="currentSelectedFormHelper === formHelperKeys.responseBodyHtml"
            :preset-response-body-type="responseBodyTypes.html"
          />
          <ResponseBodyHelper
            v-if="
              currentSelectedFormHelper === formHelperKeys.responseBodyBase64
            "
            :preset-response-body-type="responseBodyTypes.base64"
          />
          <RedirectSelector
            v-if="currentSelectedFormHelper === formHelperKeys.redirect"
          />
          <LineEndingSelector
            v-if="currentSelectedFormHelper === formHelperKeys.lineEndings"
          />
          <ScenarioSelector
            v-if="currentSelectedFormHelper === formHelperKeys.scenario"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { computed, onMounted, onUnmounted, ref, watch } from "vue";
import {
  formHelperKeys,
  stubFormHelpers,
  responseBodyTypes,
} from "@/constants/stubFormResources";
import HttpMethodSelector from "@/components/stub/HttpMethodSelector.vue";
import TenantSelector from "@/components/stub/TenantSelector.vue";
import HttpStatusCodeSelector from "@/components/stub/HttpStatusCodeSelector.vue";
import ResponseBodyHelper from "@/components/stub/ResponseBodyHelper.vue";
import RedirectSelector from "@/components/stub/RedirectSelector.vue";
import LineEndingSelector from "@/components/stub/LineEndingSelector.vue";
import ScenarioSelector from "@/components/stub/ScenarioSelector.vue";
import { useRoute } from "vue-router";
import { escapePressed } from "@/utils/event";
import { useStubFormStore } from "@/store/stubForm";
import { defineComponent } from "vue";

export default defineComponent({
  name: "FormHelperSelector",
  components: {
    LineEndingSelector,
    RedirectSelector,
    ResponseBodyHelper,
    HttpStatusCodeSelector,
    TenantSelector,
    HttpMethodSelector,
    ScenarioSelector,
  },
  setup() {
    const stubFormStore = useStubFormStore();
    const route = useRoute();

    // Refs
    const formHelperFilterInput = ref(null);

    // Data
    const showFormHelperItems = ref(false);
    const formHelperItems = ref();
    const formHelperFilter = ref("");

    // Methods
    const onFormHelperItemClick = (item) => {
      if (item.defaultValueMutation) {
        item.defaultValueMutation(stubFormStore);
      } else if (item.formHelperToOpen) {
        stubFormStore.openFormHelper(item.formHelperToOpen);
      }

      showFormHelperItems.value = false;
    };
    const openFormHelperList = () => {
      showFormHelperItems.value = true;
      setTimeout(() => {
        if (formHelperFilterInput.value) {
          formHelperFilterInput.value.focus();
        }
      }, 10);
    };
    const closeFormHelperAndList = () => {
      formHelperFilter.value = "";
      stubFormStore.closeFormHelper();
      showFormHelperItems.value = false;
    };

    // Computed
    const currentSelectedFormHelper = computed(
      () => stubFormStore.getCurrentSelectedFormHelper
    );
    const filteredStubFormHelpers = computed(() => {
      if (!formHelperFilter.value) {
        return stubFormHelpers;
      }
      return stubFormHelpers.filter((h) => {
        if (h.isMainItem) {
          return true;
        }

        return h.title
          .toLowerCase()
          .includes(formHelperFilter.value.toLowerCase());
      });
    });

    // Watch
    watch(currentSelectedFormHelper, (formHelper) => {
      if (!formHelper) {
        showFormHelperItems.value = false;
      }
    });
    watch(
      () => route.params,
      () => closeFormHelperAndList()
    );

    // Lifecycle
    const escapeListener = (e) => {
      if (escapePressed(e)) {
        e.preventDefault();
        closeFormHelperAndList();
      }
    };
    onMounted(() => document.addEventListener("keydown", escapeListener));
    onUnmounted(() => document.removeEventListener("keydown", escapeListener));

    return {
      formHelperItems,
      formHelperKeys,
      currentSelectedFormHelper,
      showFormHelperItems,
      filteredStubFormHelpers,
      onFormHelperItemClick,
      formHelperFilter,
      formHelperFilterInput,
      openFormHelperList,
      closeFormHelperAndList,
      responseBodyTypes,
    };
  },
});
</script>

<style scoped>
label {
  display: block;
  cursor: pointer;
}

.subtitle {
  font-size: 0.9em;
}
</style>
