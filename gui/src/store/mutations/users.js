import { clearUserToken, saveUserToken } from "@/utils/sessionUtil";

export function storeUserToken(state, token) {
  state.userToken = token;
  if (!token) {
    clearUserToken();
  } else {
    saveUserToken(token);
  }
}
