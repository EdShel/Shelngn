import axios from "axios";
import { refresh, saveAuthenticationTokens } from "./auth/reducer";
import { getAccessToken, getRefreshToken } from "./auth/selectors";
import { apiUrl } from "./constants";
import store from "./redux/store";

const api = axios.create({
  baseURL: apiUrl,
});
api.interceptors.request.use(
  (requestConfig) => {
    const accessToken = getAccessToken(store.getState());
    requestConfig.headers.Authorization = `Bearer ${accessToken}`;
  },
  async (error) => {
    const originalRequest = error.config;
    if (error.response.status === 401 && !originalRequest.retryAfterRefresh) {
      originalRequest.retryAfterRefresh = true;

      const refreshResult = await store.dispatch(refresh()).unwrap();
      if (refreshResult.status === 200) {
        return api(originalRequest);
      }
    }
    return Promise.reject(error);
  }
);
const apiAnonymous = axios.create({
  baseURL: apiUrl,
});

// --------- Auth --------- //
export const postRegister = ({ email, userName, password }) =>
  apiAnonymous.post("/auth/register", { email, userName, password });
export const postLogin = ({ email, password }) =>
  apiAnonymous.post("/auth/login", { email, password });
export const postRefresh = () =>
  api.post("/auth/refresh", {
    refreshToken: getRefreshToken(store.getState()),
  });
