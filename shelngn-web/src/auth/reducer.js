import { createAsyncThunk } from "@reduxjs/toolkit";
import { postLogin, postRefresh } from "../api";
import AppStorage from "../AppStorage";

const initialState = {
  accessToken: AppStorage.accessToken,
  refreshToken: AppStorage.refreshToken,
  loading: false,
  error: null,
};

const reducer = (state = initialState, action) => {
  switch (action.type) {
    case login.fulfilled:
    case refresh.fulfilled:
      console.log(action);
      console.log(action.payload);
      console.log(action.payload.data);
      return {
        ...state,
        accessToken: action.payload.data.accessToken,
        refreshToken: action.payload.data.refreshToken,
      };
    default:
      return state;
  }
};
export default reducer;

export const login = createAsyncThunk("auth/login", ({ email, password }) =>
  postLogin({ email, password })
);
export const refresh = createAsyncThunk("auth/refresh", postRefresh);
