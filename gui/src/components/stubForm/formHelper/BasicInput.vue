<template>
  <div class="row">
    <div class="col-md-12">
      <strong>{{ title }}</strong>
      <input
        v-if="inputType === 'text'"
        type="text"
        class="form-control mt-2"
        v-model="value"
        @keyup.enter="insert"
        ref="fieldRef"
      />
      <input
        v-if="inputType === 'numeric'"
        type="number"
        class="form-control mt-2"
        v-model="value"
        @keyup.enter="insert"
        ref="fieldRef"
      />
      <button class="btn btn-success mt-2" @click="insert">
        {{ buttonText ?? "Insert into stub" }}
      </button>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, onMounted, ref } from "vue";
import { useStubFormStore } from "@/store/stubForm";

export default defineComponent({
  props: {
    valueGetter: {
      type: Function,
      required: true,
    },
    valueSetter: {
      type: Function,
      required: true,
    },
    title: {
      type: String,
    },
    buttonText: {
      type: String,
    },
    inputType: {
      type: String,
      default: "text",
      validator(value: string) {
        return ["text", "numeric"].includes(value);
      },
    },
  },
  setup(props) {
    const stubFormStore = useStubFormStore();

    // Data
    const value = ref("");
    const fieldRef = ref<HTMLFormElement>();

    // Methods
    const insert = () => {
      if (props.valueSetter) {
        props.valueSetter(value.value);
      }
      stubFormStore.closeFormHelper();
    };

    // Lifecycle
    onMounted(() => {
      if (props.valueGetter) {
        value.value = props.valueGetter();
      }

      fieldRef.value?.focus();
    });

    return {
      value,
      insert,
      fieldRef,
    };
  },
});
</script>

<style scoped></style>
