<template>
  <div class="row stub">
    <div class="col-12">
      <strong class="url" v-on:click="showOrHide">{{fullStub.stub.id}}</strong>
      <a
        class="btn btn-danger pull-right"
        v-on:click="deleteStub"
        title="Delete stub"
        v-if="!fullStub.metadata.readOnly"
      >
        <span class="fa fa-trash">&nbsp;</span>
      </a>
      <a
        class="btn btn-primary pull-right"
        v-on:click="updateStub"
        title="Update stub"
        v-if="!fullStub.metadata.readOnly"
      >
        <span class="fa fa-pencil">&nbsp;</span>
      </a>
      <div class="row" v-if="visible">
        <div class="col-12">
          <router-link
            :to="{ name: 'requests', query: { searchTerm: fullStub.stub.id }}"
          >View requests made for this stub</router-link>
        </div>
        <div class="col-12">
          <pre><code>{{fullStub.stub | yaml}}</code></pre>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import resources from "@/resources";

export default {
  name: "stub",
  props: ["fullStub"],
  data() {
    return {
      visible: false
    };
  },
  created() {},
  methods: {
    showOrHide() {
      this.visible = !this.visible;
    },
    deleteStub() {
      this.$dialog.confirm(resources.areYouSure).then(() => {
        this.$store.dispatch("deleteStub", { stubId: this.fullStub.stub.id });
      });
    },
    updateStub() {
      this.$router.push({
        name: "updateStub",
        params: { stubId: this.fullStub.stub.id }
      });
    }
  }
};
</script>

<style scoped>
.stub {
  margin: 10px;
  padding: 10px;
  text-align: left;
}
.url {
  cursor: pointer;
}
.btn {
  margin-left: 5px;
}
</style>