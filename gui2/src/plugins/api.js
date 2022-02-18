import { addBeforeSendHandler } from "@/utils/api";
import { useUsersStore } from "@/store/users";

addBeforeSendHandler((url, request) => {
  const usersStore = useUsersStore();
  const token = usersStore.getUserToken;
  const headerKeys = Object.keys(request.headers);
  if (token && !headerKeys.find((k) => k.toLowerCase() === "authorization")) {
    request.headers.Authorization = `Basic ${token}`;
  }
});
