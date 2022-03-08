import { createAsyncThunk } from "@reduxjs/toolkit";
import { postLogin, postRegister, postRevoke } from "../api";
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
      AppStorage.accessToken = action.payload.accessToken;
      AppStorage.refreshToken = action.payload.refreshToken;
      return {
        ...state,
        accessToken: action.payload.accessToken,
        refreshToken: action.payload.refreshToken,
      };
    default:
      return state;
  }
};
export default reducer;

export const register = createAsyncThunk("auth/register", postRegister);
export const login = createAsyncThunk("auth/login", postLogin);
export const revokeRefreshToken = createAsyncThunk("auth/revoke", postRevoke)