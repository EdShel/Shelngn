const UrlTo = {
  home: () => "/",
  login: () => "/login",
  register: () => "/register",
  workspace: (id) => "/workspace/" + id,
  debug: (id) => "/debug/" + id,
  options: (id) => "/options/" + id,
  play: (id) => "/play/" + id,
};
export default UrlTo;
