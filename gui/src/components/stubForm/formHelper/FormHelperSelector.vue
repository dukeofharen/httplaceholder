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
  <slide-up-down v-model="showFormHelperItems" :duration="300">
    <div class="row mt-3">
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
        <div class="list-group stub-form-helpers">
          <template
            v-for="(item, index) in filteredStubFormHelpers"
            :key="index"
          >
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
  </slide-up-down>
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
          <SetBody v-if="currentSelectedFormHelper === FormHelperKey.Body" />
          <SetHeader
            v-if="currentSelectedFormHelper === FormHelperKey.Header"
          />
          <SetForm v-if="currentSelectedFormHelper === FormHelperKey.Form" />
          <SetHost v-if="currentSelectedFormHelper === FormHelperKey.Host" />
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import {
  computed,
  defineComponent,
  onMounted,
  onUnmounted,
  ref,
  watch,
} from "vue";
import HttpMethodSelector from "@/components/stubForm/formHelper/HttpMethodSelector.vue";
import TenantSelector from "@/components/stubForm/formHelper/TenantSelector.vue";
import HttpStatusCodeSelector from "@/components/stubForm/formHelper/HttpStatusCodeSelector.vue";
import ResponseBodyHelper from "@/components/stubForm/formHelper/ResponseBodyHelper.vue";
import RedirectSelector from "@/components/stubForm/formHelper/RedirectSelector.vue";
import LineEndingSelector from "@/components/stubForm/formHelper/LineEndingSelector.vue";
import ScenarioSelector from "@/components/stubForm/formHelper/ScenarioSelector.vue";
import SetDynamicMode from "@/components/stubForm/formHelper/SetDynamicMode.vue";
import SetPath from "@/components/stubForm/formHelper/SetPath.vue";
import SetFullPath from "@/components/stubForm/formHelper/SetFullPath.vue";
import { useRoute } from "vue-router";
import { escapePressed } from "@/utils/event";
import { useStubFormStore } from "@/store/stubForm";
import {
  type StubFormHelper,
  StubFormHelperCategory,
  stubFormHelpers,
} from "@/domain/stubForm/stub-form-helpers";
import { FormHelperKey } from "@/domain/stubForm/form-helper-key";
import { ResponseBodyType } from "@/domain/stubForm/response-body-type";
import ExampleSelector from "@/components/stubForm/formHelper/ExampleSelector.vue";
import SetQuery from "@/components/stubForm/formHelper/SetQuery.vue";
import SetBody from "@/components/stubForm/formHelper/SetBody.vue";
import SetHeader from "@/components/stubForm/formHelper/SetHeader.vue";
import SetForm from "@/components/stubForm/formHelper/SetForm.vue";
import SetHost from "@/components/stubForm/formHelper/SetHost.vue";

export default defineComponent({
  name: "FormHelperSelector",
  components: {
    SetHost,
    SetForm,
    SetHeader,
    SetBody,
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
    const formHelperButtons = [
      { title: "Add example", category: StubFormHelperCategory.Examples },
      {
        title: "Add general stub info",
        category: StubFormHelperCategory.GeneralInfo,
      },
      {
        title: "Add request condition",
        category: StubFormHelperCategory.RequestCondition,
      },
      {
        title: "Add response writer",
        category: StubFormHelperCategory.ResponseDefinition,
      },
    ];
    const selectedFormHelperCategory = ref<StubFormHelperCategory>(
      StubFormHelperCategory.None,
    );
    const formHelperItems = ref();

    // Methods
    const onFormHelperItemClick = (item: StubFormHelper) => {
      if (item.defaultValueMutation) {
        item.defaultValueMutation(stubFormStore);
        stubFormStore.closeFormHelper();
      } else if (item.formHelperToOpen) {
        stubFormStore.openFormHelper(item.formHelperToOpen);
      }

      showFormHelperItems.value = false;
      formHelperFilter.value = "";
    };
    const openFormHelperList = (category: StubFormHelperCategory) => {
      if (selectedFormHelperCategory.value === category) {
        closeFormHelperAndList();
        return;
      }

      showFormHelperItems.value = true;
      selectedFormHelperCategory.value = category;
      const formHelpers = stubFormHelpers.filter(
        (h) => h.stubFormHelperCategory === category,
      );
      if (formHelpers.length === 1) {
        onFormHelperItemClick(formHelpers[0]);
      } else {
        setTimeout(() => {
          if (formHelperFilterInput.value) {
            formHelperFilterInput.value.focus();
          }
        }, 10);
      }
    };
    const closeFormHelperAndList = () => {
      formHelperFilter.value = "";
      stubFormStore.closeFormHelper();
      showFormHelperItems.value = false;
      selectedFormHelperCategory.value = StubFormHelperCategory.None;
    };

    // Computed
    const currentSelectedFormHelper = computed(
      () => stubFormStore.getCurrentSelectedFormHelper,
    );
    const filteredStubFormHelpers = computed(() => {
      let result = stubFormHelpers;
      if (selectedFormHelperCategory.value) {
        result = result.filter(
          (r) => r.stubFormHelperCategory === selectedFormHelperCategory.value,
        );
      }

      if (!formHelperFilter.value) {
        return result;
      }

      return result.filter((h) => {
        return (
          !h.isHeading &&
          h.title.toLowerCase().includes(formHelperFilter.value.toLowerCase())
        );
      });
    });
    const formHelperFilter = computed({
      get: () => stubFormStore.getFormHelperSelectorFilter,
      set: (value) => stubFormStore.setFormHelperSelectorFilter(value),
    });

    // Watch
    watch(currentSelectedFormHelper, (formHelper) => {
      if (!formHelper) {
        showFormHelperItems.value = false;
      }
    });
    watch(
      () => route.params,
      () => closeFormHelperAndList(),
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
      formHelperButtons,
      selectedFormHelperCategory,
    };
  },
});
</script>

<style scoped lang="scss">
@import "@/style/bootstrap";

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
