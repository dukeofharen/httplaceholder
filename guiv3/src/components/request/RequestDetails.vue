<template>
  <div>
    <div class="row">
      <div class="col-md-12 mb-3">
        <label>URL</label>
        <span>{{ requestParams.url }}</span>
      </div>
      <div class="col-md-12 mb-3">
        <label>Client IP</label>
        <span>{{ requestParams.clientIp }}</span>
      </div>
      <div class="col-md-12 mb-3">
        <label>Correlation ID</label>
        <span>{{ request.correlationId }}</span>
      </div>
      <div class="col-md-12 mb-3">
        <label>Executed stub</label>
        <!-- TODO Add router link here -->
        <span>{{ request.executingStubId }}</span>
      </div>
      <div class="col-md-12 mb-3">
        <label>Stub tenant (category)</label>
        <!-- TODO Add router link here -->
        <span>{{ request.stubTenant }}</span>
      </div>
      <div class="col-md-12 mb-3">
        <label>Request time</label>
        <span>{{ requestTime }} (it took {{ duration }} ms)</span>
      </div>
    </div>
  </div>
</template>

<script>
import { computed } from "vue";
import { formatDateTime, getDuration } from "@/utils/datetime";

export default {
  name: "RequestDetails",
  props: {
    request: {
      type: Object,
      required: true,
    },
  },
  setup(props) {
    // Computed
    const requestParams = computed(
      () => props.request?.requestParameters || {}
    );
    const requestTime = computed(() =>
      formatDateTime(requestParams.value.requestEndTime)
    );
    const duration = computed(() => {
      const req = props.request;
      return getDuration(req.requestBeginTime, req.requestEndTime);
    });

    return { requestParams, requestTime, duration };
  },
};
</script>

<style lang="scss" scoped>
label {
  display: block;
  font-weight: bold;
}
</style>
