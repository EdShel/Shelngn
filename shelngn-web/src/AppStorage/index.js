const AppStorage = {
  get accessToken() {
    return sessionStorage.getItem("accessToken");
  },
  set accessToken(value) {
    sessionStorage.setItem("accessToken", value);
  },
  get refreshToken() {
    return sessionStorage.getItem("refreshToken");
  },
  set refreshToken(value) {
    sessionStorage.setItem("refreshToken", value);
  },
};
export default AppStorage;
