import { createAsyncThunk } from "@reduxjs/toolkit";
import { getMyGameProjects } from "../api";

const initialState = {
  myGameProjects: {
    data: null,
    loading: false,
    error: false,
  },
};

const reducer = (state = initialState, action) => {
  switch (action.type) {
    case loadMyGameProjects.pending.type:
      return {
        ...state,
        myGameProjects: {
          ...state.getMyGameProjects,
          loading: true,
          error: false,
        },
      };
    case loadMyGameProjects.fulfilled.type:
        console.log('action', action)
      return {
        ...state,
        myGameProjects: {
          ...state.myGameProjects,
          data: action.payload.gameProjects,
          loading: false,
        },
      };
    case loadMyGameProjects.rejected.type:
      return {
        ...state,
        myGameProjects: {
          ...state.myGameProjects,
          error: true,
          loading: false,
        },
      };
    default:
      return state;
  }
};
export default reducer;

export const loadMyGameProjects = createAsyncThunk("home/myGameProjects", getMyGameProjects);
