<template>
  <div class="row stub">
      <div class="col-12">
        <strong class="url" v-on:click="showOrHide">{{stub.id}}</strong>
        <div class="row" v-if="visible">
            <div class="col-12">
              <router-link :to="{ name: 'requests', query: { searchTerm: stub.id }}">View requests made for this stub</router-link>
            </div>
            <div class="col-12">
                <pre><code>{{stub | yaml}}</code></pre>
            </div>
        </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "stub",
  props: ["stub"],
  data() {
    return {
      visible: false
    };
  },
  created() {},
  methods: {
    showOrHide() {
      this.visible = !this.visible;
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
</style>