<template>
  <v-row>
    <v-col>
      <v-list-item v-if="!showList">
        <v-list-item-content>
          <v-list-item-title class="clickable bold" @click="showList = !showList">Click here to add request or response value</v-list-item-title>
        </v-list-item-content>
      </v-list-item>
      <template v-if="showList">
        <v-list-item v-for="(item, index) in formHelperItems" :key="index">
          <v-list-item-content>
            <v-list-item-title v-if="item.onClick" @click="item.onClick" class="clickable">{{
                item.title
              }}
            </v-list-item-title>
            <v-list-item-title :class="{bold: item.divider}" v-else>{{ item.title }}</v-list-item-title>
            <v-list-item-subtitle v-if="item.subTitle">{{item.subTitle}}</v-list-item-subtitle>
          </v-list-item-content>
        </v-list-item>
      </template>
    </v-col>
  </v-row>
</template>

<script>
import {tooltipResources} from "@/shared/stubFormResources";

export default {
  mounted() {

  },
  data() {
    return {
      showList: false,
      formHelperItems: [
        {
          title: "Request",
          divider: true
        },
        {
          title: "Description",
          subTitle: tooltipResources.description,
          onClick: this.setDescription
        },
        {
          title: "Priority",
          subTitle: tooltipResources.priority,
          onClick: this.setPriority
        }
      ]
    };
  },
  methods: {
    setDescription() {
      this.$store.commit("stubForm/setDefaultDescription");
      this.showList = false;
    },
    setPriority() {
      this.$store.commit("stubForm/setDefaultPriority");
      this.showList = false;
    }
  }
};
</script>

<style scoped>
.bold {
  font-weight: bold;
}

.clickable {
  cursor: pointer;
}
</style>
