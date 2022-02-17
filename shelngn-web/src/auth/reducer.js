import { createAsyncThunk } from "@reduxjs/toolkit";
import { postLogin, postRefresh, postRegister } from "../api";
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
      console.log('CAAAALLLL?>>>>???')
      console.log(action);
      console.log(action.payload);
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
console.log('red1', reducer)
export default reducer;

export const register = createAsyncThunk("auth/register", postRegister);
export const login = createAsyncThunk("auth/login", postLogin);
