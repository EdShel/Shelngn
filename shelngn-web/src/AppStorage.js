const AppStorage = {
  get accessToken() {
    return localStorage.getItem("accessToken");
  },
  set accessToken(value) {
    localStorage.setItem("accessToken", value);
  },
  get refreshToken() {
    return localStorage.getItem("refreshToken");
  },
  set refreshToken(value) {
    localStorage.setItem("refreshToken", value);
  },
};
export default AppStorage;
