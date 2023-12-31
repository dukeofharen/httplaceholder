<template>
  <div class="row">
    <div class="col-md-12">
      <strong>Insert description</strong>
      <input
        type="text"
        class="form-control mt-2"
        v-model="value"
        @keyup.enter="insert"
      />
      <button class="btn btn-success mt-2" @click="insert">Add</button>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent, onMounted, ref, watch } from "vue";
import { useStubFormStore } from "@/store/stubForm";

export default defineComponent({
  props: {
    valueGetter: {
      type: Function,
    },
    valueSetter: {
      type: Function,
    },
  },
  setup(props) {
    const stubFormStore = useStubFormStore();

    // Data
    const value = ref("");

    // Methods
    const insert = () => {
      stubFormStore.closeFormHelper();
    };

    // Watches
    watch(value, (newValue) => {
      if (newValue && props.valueSetter) {
        props.valueSetter(newValue);
      }
    });

    // Lifecycle
    onMounted(() => {
      if (props.valueGetter) {
        value.value = props.valueGetter();
      }
    });

    return {
      value,
      insert,
    };
  },
});
</script>

<style scoped></style>
