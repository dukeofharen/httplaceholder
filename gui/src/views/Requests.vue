<template>
  <div class="home">
    <Request v-bind:request="request" v-for="request in requests" :key="request.correlationId"></Request>
  </div>
</template>

<script>
import { getRequests } from "@/data/serviceAgent";
import Request from '@/components/Request'

export default {
  name: "requests",
  data() {
    return {
      requests: []
    };
  },
  components: {
      Request
  },
  created() {
    getRequests()
      .then(response => {
        this.requests = response.data;
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