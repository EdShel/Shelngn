import { configureStore } from "@reduxjs/toolkit";
import workspace from "./WorkspacePage/reducer";

const store = configureStore({
  reducer: {
    workspace,
  },
});
export default store;
