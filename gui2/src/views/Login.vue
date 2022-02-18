<template>
  <div @keyup.enter="logIn">
    <h1>Log in</h1>

    <div class="row">
      <div class="col-md-12 input-group">
        <input
          type="text"
          class="form-control"
          placeholder="Username"
          v-model="username"
        />
      </div>
    </div>

    <div class="row mt-3">
      <div class="col-md-12 input-group">
        <input
          type="password"
          class="form-control"
          placeholder="Password"
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
          Log in
        </button>
      </div>
    </div>
  </div>
</template>

<script>
import { computed, ref } from "vue";
import { resources } from "@/constants/resources";
import { handleHttpError } from "@/utils/error";
import { useRouter } from "vue-router";
import { error } from "@/utils/toast";
import { useUsersStore } from "@/store/users";

export default {
  name: "Login",
  setup() {
    const userStore = useUsersStore();
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
        });
        await router.push({ name: "Requests" });
      } catch (e) {
        if (e.status === 401) {
          error(resources.credentialsIncorrect);
        } else {
          handleHttpError(e);
        }
      }
    };

    return { username, password, logIn, buttonEnabled };
  },
};
</script>

<style scoped></style>
