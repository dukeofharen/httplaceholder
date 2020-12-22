import store from "@/store";

export default function addTokenToRequest(request) {
  const token = store.getters["users/getUserToken"];
  if (!!token && !request.headers["Authorization"]) {
    request.headers["Authorization"] = `Basic ${token}`;
  }

  return request;
}
