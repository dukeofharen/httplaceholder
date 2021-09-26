<template>
  <h1>{{ title }}</h1>

  <div class="row">
    <div class="col-md-12">
      Fill in the stub below in YAML format and click on "Save". For examples,
      visit
      <a href="https://github.com/dukeofharen/httplaceholder" target="_blank"
        >https://github.com/dukeofharen/httplaceholder</a
      >.
    </div>
  </div>

  <div class="row">
    <div class="col-md-12">
      <codemirror v-model="input" :options="cmOptions" />
      {{ input }}
    </div>
  </div>
</template>

<script>
import { useRoute } from "vue-router";
import { computed, ref } from "vue";
import { useStore } from "vuex";

export default {
  name: "StubForm",
  setup() {
    const route = useRoute();
    const store = useStore();

    // Data
    const stubId = ref(route.params.stubId);
    // const input = ref(""); // TODO move this to store lateron.
    const cmOptions = {
      tabSize: 4,
      mode: "text/x-yaml",
      lineNumbers: true,
      line: true,
    };

    // Computed
    const newStub = computed(() => !stubId.value);
    const title = computed(() => (newStub.value ? "Add stub" : "Update stub"));
    const input = computed({
      get: () => store.getters["stubForm/getInput"],
      set: (value) => store.commit("stubForm/setInput", value),
    });

    return { stubId, newStub, title, input, cmOptions };
  },
};
</script>

<style scoped></style>
