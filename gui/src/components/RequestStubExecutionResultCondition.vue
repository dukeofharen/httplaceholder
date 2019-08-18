<template>
  <div class="col-12 row condition">
    <div class="col-2">Checker name</div>
    <div class="col-10">{{condition.checkerName}}</div>

    <div class="col-2">Condition validation</div>
    <div class="col-10">
      <Bool
        v-bind:bool="condition.conditionValidation === conditionValidationType.Valid"
        trueText="Valid"
        falseText="Invalid"
      />
    </div>

    <div class="col-2">Log</div>
    <div class="col-10">{{condition.log}}</div>
  </div>
</template>

<script>
import Bool from "./Bool";
import { conditionValidationType } from "@/resources";

export default {
  name: "requestStubExecutionResultCondition",
  data() {
    return {
      conditionValidationType
    }
  },
  props: ["condition"],
  components: {
    Bool
  },
  created() {}
};
</script>

<style scoped>
.condition {
  margin-bottom: 15px;
}
</style>