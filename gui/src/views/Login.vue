<template>
  <div @keyup.enter="logIn">
    <h1>{{ $translate("logIn.logIn") }}</h1>

    <div class="row">
      <div class="col-md-12 input-group">
        <input
          type="text"
          class="form-control"
          :placeholder="$translate('general.username')"
          v-model="username"
        />
      </div>
    </div>

    <div class="row mt-3">
      <div class="col-md-12 input-group">
        <input
          type="password"
          class="form-control"
          :placeholder="$translate('general.password')"
          v-model="password"
        />
      </div>
    </div>

    <div class="row mt-3">
      <div class="col-md-12">
        <button
          class="btn btn-primary"
          @click="logIn"
          :disabled="!buttonEnabled"
        >
          {{ $translate("logIn.logIn") }}
        </button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { computed, onMounted, ref } from "vue";
import { resources } from "@/constants/resources";
import { handleHttpError } from "@/utils/error";
import { useRouter } from "vue-router";
import { error } from "@/utils/toast";
import { type AuthenticationInput, useUsersStore } from "@/store/users";
import { defineComponent } from "vue";
import { useMetadataStore } from "@/store/metadata";

export default defineComponent({
  name: "Login",
  setup() {
    const userStore = useUsersStore();
    const metadataStore = useMetadataStore();
    const router = useRouter();

    // Data
    const username = ref("");
    const password = ref("");

    // Computed
    const buttonEnabled = computed(() => !!username.value && !!password.value);

    // Methods
    const logIn = async () => {
      if (!buttonEnabled.value) {
        return;
      }

      try {
        await userStore.authenticate({
          username: username.value,
          password: password.value,
        } as AuthenticationInput);
        await router.push({ name: "Requests" });
      } catch (e: any) {
        if (e.status === 401) {
          error(resources.credentialsIncorrect);
        } else {
          handleHttpError(e);
        }
      }
    };

    // Lifecycle
    onMounted(async () => {
      if (
        !metadataStore.getAuthenticationEnabled ||
        userStore.getAuthenticated
      ) {
        await router.push({ name: "Requests" });
      }
    });

    return { username, password, logIn, buttonEnabled };
  },
});
</script>

<style scoped></style>
