import { configureStore } from "@reduxjs/toolkit";
import auth from "./auth/reducer";

const store = configureStore({
  reducer: {
    auth,
  },
});
export default store;
