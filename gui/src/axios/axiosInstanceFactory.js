import axios from 'axios/dist/axios';
import urls from "urls";
import handleError from '@/axios/errorInterceptor';

export default function createInstance() {
    let instance = axios.create({
        baseURL: urls.rootUrl,
        params: {}
    });
    instance.interceptors.response.use(
        response => response,
        error => handleError(error)
    );
    // instance.interceptors.response.use(response => handleJwtRenewal(response));
    // instance.interceptors.request.use(request => addJwtToRequest(request));
    return instance;
}
