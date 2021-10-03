import { addBeforeSendHandler } from "@/utils/api";
import store from "@/store";

addBeforeSendHandler((url, request) => {
  const token = store.getters["users/getUserToken"];
  const headerKeys = Object.keys(request.headers);
  console.log(headerKeys);
  if (token && !headerKeys.find((k) => k.toLowerCase() === "authorization")) {
    request.headers.Authorization = `Basic ${token}`;
  }
});
