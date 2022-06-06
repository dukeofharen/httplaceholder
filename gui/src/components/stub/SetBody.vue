<template>
  <div class="row">
    <div class="col-md-12 mt-3" v-if="keywords.length">
      <strong>Select a way to check the request body</strong>
      <div class="list-group mt-2">
        <button
          v-for="(keyword, index) of keywords"
          :key="index"
          class="list-group-item list-group-item-action fw-bold"
          @click="keywordSelected(keyword)"
        >
          <strong class="mb-1">{{ keyword.name }}</strong
          ><br />
          <small>{{ keyword.description }}</small>
        </button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import {
  getStringCheckingKeywords,
  type StringCheckingKeyword,
} from "@/constants/string-checking-keywords";
import { useStubFormStore } from "@/store/stubForm";

export default defineComponent({
  name: "SetBody",
  setup() {
    const stubFormStore = useStubFormStore();

    // Methods
    const keywordSelected = (keyword: StringCheckingKeyword) => {
      stubFormStore.setDefaultRequestBody(keyword);
      stubFormStore.closeFormHelper();
    };

    return { keywords: getStringCheckingKeywords(false), keywordSelected };
  },
});
</script>
