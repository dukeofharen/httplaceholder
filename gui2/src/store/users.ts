import { getUserToken, clearUserToken, saveUserToken } from "@/utils/session";
import { defineStore } from "pinia";
import { toBase64 } from "@/utils/text";
import { get } from "@/utils/api";

type UserState = {
  userToken?: string;
};

export interface AuthenticationInput {
  username: string;
  password: string;
}

const token = getUserToken();

export const useUsersStore = defineStore({
  id: "users",
  state: () =>
    ({
      userToken: token || "",
    } as UserState),
  getters: {
    getAuthenticated: (state) => !!state.userToken,
    getUserToken: (state) => state.userToken,
  },
  actions: {
    async authenticate(input: AuthenticationInput): Promise<any> {
      const username = input.username;
      const password = input.password;
      const token = toBase64(`${username}:${password}`);
      try {
        const response = await get(`/ph-api/users/${username}`, {
          headers: {
            Authorization: `Basic ${token}`,
          },
        });
        if (!token) {
          clearUserToken();
        } else {
          saveUserToken(token);
        }

        this.userToken = token;
        return Promise.resolve(response);
      } catch (e) {
        return Promise.reject(e);
      }
    },
    logOut(): void {
      clearUserToken();
      this.userToken = "";
      document.cookie = `HttPlaceholderLoggedin=;expires=${new Date(
        0
      ).toUTCString()}`;
    },
  },
});
