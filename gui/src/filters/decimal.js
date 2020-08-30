import Vue from "vue";

Vue.filter("decimal", value => new Intl.NumberFormat().format(value));
