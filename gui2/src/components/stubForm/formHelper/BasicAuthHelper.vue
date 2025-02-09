<template>
  <div class="row" @keyup.enter="insert">
    <div class="col-md-12">
      <strong>{{ $translate("stubForm.basicAuthTitle") }}</strong>
      <input
        type="text"
        class="form-control mt-2"
        v-model="username"
        :placeholder="$translate('general.username')"
        ref="usernameRef"
      />
      <input
        type="text"
        class="form-control mt-2"
        v-model="password"
        :placeholder="$translate('general.password')"
      />
      <button class="btn btn-success mt-2" @click="insert">
        {{ $translate("stubForm.insertIntoStub") }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useStubFormStore } from "@/store/stubForm";
import { onMounted, ref } from "vue";

const stubFormStore = useStubFormStore();

// Data
const usernameRef = ref<HTMLElement>();
const username = ref("");
const password = ref("");

// Methods
function insert() {
  stubFormStore.setBasicAuth(username.value, password.value);
  stubFormStore.closeFormHelper();
}

// Lifecycle
onMounted(() => {
  const basicAuth = stubFormStore.getBasicAuth;
  if (basicAuth) {
    username.value = basicAuth.username ?? "";
    password.value = basicAuth.password ?? "";
  }

  usernameRef.value?.focus();
});
</script>

<style scoped></style>
