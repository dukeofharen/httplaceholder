import { addBeforeSendHandler } from "@/utils/api";
import { useUsersStore } from "@/store/users";

addBeforeSendHandler((url: string, request: RequestInit) => {
  const usersStore = useUsersStore();
  const token = usersStore.getUserToken;
  const headerKeys = Object.keys(request.headers as object);
  if (token && !headerKeys.find((k) => k.toLowerCase() === "authorization")) {
    (request.headers as any).Authorization = `Basic ${token}`;
  }
});
