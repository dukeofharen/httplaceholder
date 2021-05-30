<template>
  <span v-bind:class="{ 'green-bool': bool, 'red-bool': !bool }">{{
    bool ? trueTextValue : falseTextValue
  }}</span>
</template>

<script>
export default {
  name: "bool",
  props: ["bool", "trueText", "falseText"],
  data() {
    return {
      trueTextValue: "",
      falseTextValue: ""
    };
  },
  created() {
    if (!this.trueText) {
      this.trueTextValue = "true";
    } else {
      this.trueTextValue = this.trueText;
    }

    if (!this.falseText) {
      this.falseTextValue = "false";
    } else {
      this.falseTextValue = this.falseText;
    }
  }
};
</script>

<style scoped>
.green-bool {
  color: #4caf50;
}
.red-bool /*gives you wings*/ {
  color: #ff5252;
}
</style>
