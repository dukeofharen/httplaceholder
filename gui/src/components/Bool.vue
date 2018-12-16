<template>
  <span v-bind:class="{'green': bool, 'red': !bool}">{{bool ? trueTextValue : falseTextValue}}</span>
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
</style>