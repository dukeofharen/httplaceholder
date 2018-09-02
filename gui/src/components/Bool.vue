<template>
  <span v-bind:class="{'true': bool, 'false': !bool}">{{bool ? trueTextValue : falseTextValue}}</span>
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
.true {
  color: #00d61d;
}
.false {
  color: #d10000;
}
</style>