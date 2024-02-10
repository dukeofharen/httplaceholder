<template>
  <div class="row" @keyup.enter="insert">
    <div class="col-md-12">
      <strong>{{ props.title }}</strong>
      <div v-for="(item, index) of formData" :key="index" class="row mt-2">
        <div class="col-md-3">
          <select
            class="form-select"
            v-model="formData[index].stringCheckingKeyword"
          >
            <option
              v-for="keyword of stringCheckingKeywords"
              :key="keyword.key"
              :value="keyword.key"
            >
              {{ keyword.name }} ({{ keyword.description }})
            </option>
          </select>
        </div>
        <div class="col-md-3">
          <input
            type="text"
            class="form-control"
            v-model="formData[index].value"
          />
        </div>
      </div>
      <div class="row">
        <div class="col-md-12">
          <button class="btn btn-success mt-2" @click="insert">
            {{ buttonText ?? "Add" }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from "vue";
import {
  getStringCheckingKeywords,
  keywords,
} from "@/constants/string-checking-keywords";
import { useStubFormStore } from "@/store/stubForm";

const props = defineProps({
  valueGetter: {
    type: Function,
    required: true,
  },
  valueSetter: {
    type: Function,
    required: true,
  },
  title: String,
  buttonText: String,
  multiple: Boolean,
});
const stubFormStore = useStubFormStore();

interface FormData {
  stringCheckingKeyword: string;
  value: string;
}

// Data
const stringCheckingKeywords = getStringCheckingKeywords(false);
const formData = ref<FormData[]>([]);

// Methods
function addEmptyEntry() {
  formData.value.push({
    value: "",
    stringCheckingKeyword: keywords.equals,
  });
}

const insert = () => {
  if (props.valueSetter) {
    const result: any = {};
    for (const item of formData.value) {
      result[item.stringCheckingKeyword] = item.value;
    }

    props.valueSetter(result);
  }

  stubFormStore.closeFormHelper();
};

// Lifecycle
onMounted(() => {
  if (props.valueGetter) {
    const val = props.valueGetter();
    if (val) {
      if (typeof val === "string") {
        formData.value.push({
          value: val,
          stringCheckingKeyword: keywords.regex,
        });
      } else {
        const keys = Object.keys(val);
        for (const key of keys) {
          const stringCheckingValue = val[key];
          formData.value.push({
            value: stringCheckingValue,
            stringCheckingKeyword: key,
          });
        }
      }
    } else {
      addEmptyEntry();
    }
  } else {
    addEmptyEntry();
  }
});
</script>
