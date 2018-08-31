<template>
  <div class="col-12 row result">
        <div class="col-2">Stub ID</div>
        <div class="col-10">{{executionResult.stubId}}</div>

        <div class="col-2">Passed</div>
        <div class="col-10">{{executionResult.passed}}</div>

        <div class="col-12">
            <strong class="url" v-on:click="showConditions">Conditions</strong>
        </div>
        <RequestStubExecutionResultCondition v-if="conditionsVisible" v-for="(condition, key) in executionResult.conditions" v-bind:condition="condition" :key="key"></RequestStubExecutionResultCondition>

        <div class="col-12">
            <strong class="url" v-on:click="showNegativeConditions">Negative conditions</strong>
        </div>
        <RequestStubExecutionResultCondition v-if="negativeConditionsVisible" v-for="(condition, key) in executionResult.negativeConditions" v-bind:condition="condition" :key="key"></RequestStubExecutionResultCondition>

        <div class="col-12">
            <hr />
        </div>
    </div>
</template>

<script>
import RequestStubExecutionResultCondition from './RequestStubExecutionResultCondition'

export default {
  name: "requestStubExecutionResult",
  props: ["executionResult"],
  components: {
      RequestStubExecutionResultCondition
  },
  data() {
      return {
          conditionsVisible: false,
          negativeConditionsVisible: false
      }
  },
  created() {},
  methods: {
      showConditions() {
          this.conditionsVisible = !this.conditionsVisible
      },
      showNegativeConditions() {
          this.negativeConditionsVisible = !this.negativeConditionsVisible
      }
  }
};
</script>

<style scoped>
.result {
    margin-bottom: 15px;
}
.url {
  cursor: pointer;
}
</style>