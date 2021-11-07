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
      <div class="input-group mb-3">
        <input
          type="text"
          class="form-control"
          placeholder="Filter form helpers (press 'Escape' to close)..."
          v-model="formHelperFilter"
          ref="formHelperFilterInput"
          @keyup.esc="closeFormHelperAndList"
        />
      </div>
      <div class="list-group">
        <template v-for="(item, index) in filteredStubFormHelpers" :key="index">
          <div v-if="item.isMainItem" class="list-group-item fw-bold fs-4">
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
import { computed, ref, watch } from "vue";
import { formHelperKeys, stubFormHelpers } from "@/constants/stubFormResources";
import { useStore } from "vuex";
import HttpMethodSelector from "@/components/stub/HttpMethodSelector";
import TenantSelector from "@/components/stub/TenantSelector";
import HttpStatusCodeSelector from "@/components/stub/HttpStatusCodeSelector";
import ResponseBodyHelper from "@/components/stub/ResponseBodyHelper";
import RedirectSelector from "@/components/stub/RedirectSelector";
import LineEndingSelector from "@/components/stub/LineEndingSelector";
import ScenarioSelector from "@/components/stub/ScenarioSelector";

export default {
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
    const store = useStore();

    // Refs
    const formHelperFilterInput = ref(null);

    // Data
    const showFormHelperItems = ref(false);
    const formHelperItems = ref();
    const formHelperFilter = ref("");

    // Methods
    const onFormHelperItemClick = (item) => {
      if (item.defaultValueMutation) {
        store.commit(item.defaultValueMutation);
      } else if (item.formHelperToOpen) {
        store.commit("stubForm/openFormHelper", item.formHelperToOpen);
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
      store.commit("stubForm/closeFormHelper");
      showFormHelperItems.value = false;
    };

    // Computed
    const currentSelectedFormHelper = computed(
      () => store.getters["stubForm/getCurrentSelectedFormHelper"]
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
    };
  },
};
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
