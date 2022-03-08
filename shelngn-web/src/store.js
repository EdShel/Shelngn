import { configureStore } from "@reduxjs/toolkit";
import auth from "./auth/reducer";
import workspace from "./WorkspacePage/reducer";
import home from "./HomePage/reducer";

const store = configureStore({
  reducer: {
    auth,
    workspace,
    home,
  },
});
export default store;
