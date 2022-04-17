const UrlTo = {
  home: () => "/",
  login: () => "/login",
  register: () => "/register",
  workspace: (id) => "/workspace/" + id,
  debug: (id) => "/debug/" + id,
};
export default UrlTo;
