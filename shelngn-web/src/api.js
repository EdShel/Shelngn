import axios from "axios";
import AppStorage from "./AppStorage";
import { apiUrl } from "./constants";

const api = axios.create({
  baseURL: apiUrl,
});
api.interceptors.request.use((requestConfig) => {
  const accessToken = AppStorage.accessToken;
  requestConfig.headers.Authorization = `Bearer ${accessToken}`;
  return requestConfig;
});
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    if (error.response.status === 401 && !originalRequest.retryAfterRefresh) {
      originalRequest.retryAfterRefresh = true;
      try {
        await postRefresh();
        return api(originalRequest);
      } catch (ex) {
        AppStorage.accessToken = null;
        AppStorage.refreshToken = null;
      }
    }
    return Promise.reject(error);
  }
);
const getAuth = async (url, config) => (await api.get(url, config)).data;
const postAuth = async (url, data, config) => (await api.post(url, data, config)).data;
const deleteAuth = async (url, config) => (await api.delete(url, config)).data;

const apiAnonymous = axios.create({
  baseURL: apiUrl,
});
const postAnonymous = async (url, data) => (await apiAnonymous.post(url, data)).data;

// --------- Auth --------- //
export const postRegister = ({ email, userName, password }) =>
  postAnonymous("/auth/register", { email, userName, password });
export const postLogin = async ({ email, password }) => {
  const data = await postAnonymous("/auth/login", { email, password });
  AppStorage.accessToken = data.accessToken;
  AppStorage.refreshToken = data.refreshToken;
};
export const postRefresh = async () => {
  const data = await postAuth("/auth/refresh", { refreshToken: AppStorage.refreshToken });
  AppStorage.accessToken = data.accessToken;
  AppStorage.refreshToken = data.refreshToken;
};
export const postRevoke = async () => {
  const revokePromise = postAuth("/auth/revoke", { refreshToken: AppStorage.refreshToken });
  AppStorage.accessToken = null;
  AppStorage.refreshToken = null;
  await revokePromise;
};
export const getCurrentUser = () => getAuth("/auth/me");

// --------- Game project --------- //
export const postCreateNewProject = () => postAuth("/gameProject");
export const deleteProject = (gameProjectId) => deleteAuth(`/gameProject/${gameProjectId}`);
export const getMyGameProjects = () => getAuth("/gameProject/my");
export const getProjectInfo = (gameProjectId) => getAuth(`/gameProject/${gameProjectId}`);
export const postAddMember = (gameProjectId, emailOrUserName) =>
  postAuth(`/gameProject/${gameProjectId}/member`, { emailOrUserName });
export const deleteMember = (gameProjectId, appUserId) =>
  deleteAuth(`/gameProject/${gameProjectId}/member/${appUserId}`);

// --------- Workspace --------- //
export const postFileUploadRequest = (workspaceId, filePath, fileContentType) =>
  postAuth(`/workspace/file/${workspaceId}/${filePath}`, null, {
    headers: {
      "Content-Type": fileContentType,
    },
  });
export const getBuiltJsBundle = (workspaceId) => getAuth(`workspace/build/${workspaceId}/bundle.js`);
export const getResourceUrl = (workspaceId, url) => `${apiUrl}/workspace/build/${workspaceId}/${url}`;
