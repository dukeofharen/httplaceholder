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
          <ExampleSelector
            v-if="currentSelectedFormHelper === FormHelperKey.Example"
          />
          <HttpMethodSelector
            v-if="currentSelectedFormHelper === FormHelperKey.HttpMethod"
          />
          <TenantSelector
            v-if="currentSelectedFormHelper === FormHelperKey.Tenant"
          />
          <HttpStatusCodeSelector
            v-if="currentSelectedFormHelper === FormHelperKey.StatusCode"
          />
          <ResponseBodyHelper
            v-if="currentSelectedFormHelper === FormHelperKey.ResponseBody"
          />
          <ResponseBodyHelper
            v-if="
              currentSelectedFormHelper === FormHelperKey.ResponseBodyPlainText
            "
            :preset-response-body-type="ResponseBodyType.text"
          />
          <ResponseBodyHelper
            v-if="currentSelectedFormHelper === FormHelperKey.ResponseBodyJson"
            :preset-response-body-type="ResponseBodyType.json"
          />
          <ResponseBodyHelper
            v-if="currentSelectedFormHelper === FormHelperKey.ResponseBodyXml"
            :preset-response-body-type="ResponseBodyType.xml"
          />
          <ResponseBodyHelper
            v-if="currentSelectedFormHelper === FormHelperKey.ResponseBodyHtml"
            :preset-response-body-type="ResponseBodyType.html"
          />
          <ResponseBodyHelper
            v-if="
              currentSelectedFormHelper === FormHelperKey.ResponseBodyBase64
            "
            :preset-response-body-type="ResponseBodyType.base64"
          />
          <RedirectSelector
            v-if="currentSelectedFormHelper === FormHelperKey.Redirect"
          />
          <LineEndingSelector
            v-if="currentSelectedFormHelper === FormHelperKey.LineEndings"
          />
          <ScenarioSelector
            v-if="currentSelectedFormHelper === FormHelperKey.Scenario"
          />
          <SetDynamicMode
            v-if="currentSelectedFormHelper === FormHelperKey.DynamicMode"
          />
          <SetPath v-if="currentSelectedFormHelper === FormHelperKey.Path" />
          <SetFullPath
            v-if="currentSelectedFormHelper === FormHelperKey.FullPath"
          />
          <SetQuery v-if="currentSelectedFormHelper === FormHelperKey.Query" />
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { computed, onMounted, onUnmounted, ref, watch } from "vue";
import HttpMethodSelector from "@/components/stub/HttpMethodSelector.vue";
import TenantSelector from "@/components/stub/TenantSelector.vue";
import HttpStatusCodeSelector from "@/components/stub/HttpStatusCodeSelector.vue";
import ResponseBodyHelper from "@/components/stub/ResponseBodyHelper.vue";
import RedirectSelector from "@/components/stub/RedirectSelector.vue";
import LineEndingSelector from "@/components/stub/LineEndingSelector.vue";
import ScenarioSelector from "@/components/stub/ScenarioSelector.vue";
import SetDynamicMode from "@/components/stub/SetDynamicMode.vue";
import SetPath from "@/components/stub/SetPath.vue";
import SetFullPath from "@/components/stub/SetFullPath.vue";
import { useRoute } from "vue-router";
import { escapePressed } from "@/utils/event";
import { useStubFormStore } from "@/store/stubForm";
import { defineComponent } from "vue";
import {
  type StubFormHelper,
  stubFormHelpers,
} from "@/domain/stubForm/stub-form-helpers";
import { FormHelperKey } from "@/domain/stubForm/form-helper-key";
import { ResponseBodyType } from "@/domain/stubForm/response-body-type";
import ExampleSelector from "@/components/stub/ExampleSelector.vue";
import SetQuery from "@/components/stub/SetQuery.vue";

export default defineComponent({
  name: "FormHelperSelector",
  components: {
    SetQuery,
    SetFullPath,
    ExampleSelector,
    LineEndingSelector,
    RedirectSelector,
    ResponseBodyHelper,
    HttpStatusCodeSelector,
    TenantSelector,
    HttpMethodSelector,
    ScenarioSelector,
    SetDynamicMode,
    SetPath,
  },
  setup() {
    const stubFormStore = useStubFormStore();
    const route = useRoute();

    // Refs
    const formHelperFilterInput = ref<HTMLElement>();

    // Data
    const showFormHelperItems = ref(false);
    const formHelperItems = ref();
    const formHelperFilter = ref("");

    // Methods
    const onFormHelperItemClick = (item: StubFormHelper) => {
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
    const escapeListener = (e: KeyboardEvent) => {
      if (escapePressed(e)) {
        e.preventDefault();
        closeFormHelperAndList();
      }
    };
    onMounted(() => document.addEventListener("keydown", escapeListener));
    onUnmounted(() => document.removeEventListener("keydown", escapeListener));

    return {
      formHelperItems,
      currentSelectedFormHelper,
      showFormHelperItems,
      filteredStubFormHelpers,
      onFormHelperItemClick,
      formHelperFilter,
      formHelperFilterInput,
      openFormHelperList,
      closeFormHelperAndList,
      FormHelperKey,
      ResponseBodyType,
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
