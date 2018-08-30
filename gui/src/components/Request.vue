<template>
  <div class="row">
    <div class="col-12 request" :key="request.correlationId">
        <strong class="url" v-on:click="showDetails">{{request.requestParameters.url}} ({{request.executingStubId ? "executed" : "not executed"}})</strong>
        <transition name="slide-fade">
            <div class="row" v-if="detailsVisible">
                <div class="col-2">Method</div>
                <div class="col-10">{{request.requestParameters.method}}</div>

                <div class="col-2">Body</div>
                <div class="col-10">{{request.requestParameters.body}}</div>

                <div class="col-2">Client IP</div>
                <div class="col-10">{{request.requestParameters.clientIp}}</div>

                <div class="col-2">Headers</div>
                <div class="col-10">
                    <ul>
                      <li v-for="(value, key) in request.requestParameters.headers" :key="key">{{key}}: {{value}}</li>
                    </ul>
                </div>

                <div class="col-2">Correlation ID</div>
                <div class="col-10">{{request.correlationId}}</div>

                <div class="col-2">Executed stub</div>
                <div class="col-10">{{request.executingStubId}}</div>

                <!-- TODO format with Moment.js -->
                <div class="col-2">Request begin time</div>
                <div class="col-10">{{request.requestBeginTime}}</div>

                <!-- TODO format with Moment.js -->
                <div class="col-2">Request end time</div>
                <div class="col-10">{{request.requestEndTime}}</div>
            </div>
        </transition>
    </div>
  </div>
</template>

<script>
export default {
  name: "request",
  props: ["request"],
  data() {
    return {
      detailsVisible: false
    };
  },
  created() {},
  methods: {
    showDetails() {
      this.detailsVisible = !this.detailsVisible;
    }
  }
};
</script>

<style scoped>
.request {
  background-color: #f8f9fa;
  margin: 10px;
  padding: 10px;
  text-align: left;
}
.url {
  cursor: pointer;
}

.slide-fade-enter-active {
  transition: all 0.3s ease;
}
.slide-fade-leave-active {
  transition: all 0.3s cubic-bezier(1, 0.5, 0.8, 1);
}
.slide-fade-enter,
.slide-fade-leave-to {
  transform: translateX(10px);
  opacity: 0;
}
</style>