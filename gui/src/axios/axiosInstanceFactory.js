import axios from "axios/dist/axios";
import urls from "urls";
import handleError from "@/axios/errorInterceptor";
import addTokenToRequest from "@/axios/tokenOnRequestInterceptor";

export default function createInstance() {
  let instance = axios.create({
    baseURL: urls.rootUrl,
    params: {}
  });
  instance.interceptors.response.use(
    response => response,
    error => handleError(error)
  );
  instance.interceptors.request.use(request => addTokenToRequest(request));
  return instance;
}
