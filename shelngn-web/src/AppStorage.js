const AppStorage = {
  get accessToken() {
    return localStorage.getItem("accessToken");
  },
  set accessToken(value) {
    if (!value) {
      localStorage.removeItem("accessToken");
    } else {
      localStorage.setItem("accessToken", value);
    }
  },
  get refreshToken() {
    return localStorage.getItem("refreshToken");
  },
  set refreshToken(value) {
    if (!value) {
      localStorage.removeItem("refreshToken");
    } else {
      localStorage.setItem("refreshToken", value);
    }
  },
};
export default AppStorage;
