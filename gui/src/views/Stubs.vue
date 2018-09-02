<template>
  <div class="stubs">
    <Stub v-bind:stub="stub" v-for="stub in stubs" :key="stub.id"></Stub>
  </div>
</template>

<script>
import { getStubs } from "@/data/serviceAgent";
import Stub from '@/components/Stub'

export default {
  name: "stubs",
  data() {
    return {
      stubs: []
    };
  },
  components: {
      Stub
  },
  created() {
    getStubs()
      .then(response => {
        this.stubs = response.data;
      })
      .catch(error => {
        // TODO show error message
        console.log(error);
      });
  }
};
</script>

<style scoped>
.request {
    background-color: #f8f9fa;
    margin: 10px;
    padding:10px;
}
</style>