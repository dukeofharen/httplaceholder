<template>
  <div class="col-12 row condition">
        <div class="col-2">Checker name</div>
        <div class="col-10">{{condition.checkerName}}</div>

        <div class="col-2">Condition validation</div>
        <div class="col-10">{{condition.ConditionValidation}}</div>

        <div class="col-2">Log</div>
        <div class="col-10">{{condition.log}}</div>
    </div>
</template>

<script>
export default {
  name: "requestStubExecutionResultCondition",
  props: ["condition"],
  created() {}
};
</script>

<style scoped>
.condition {
    margin-bottom: 15px;
}
</style>