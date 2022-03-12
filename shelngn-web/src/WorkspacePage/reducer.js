const USER_CONNECTED = "workspace/userConnected";
const USER_DISCONNECTED = "workspace/userDisconnected";

const initialState = {
  users: [],
  filesTree: null,
};

const reducer = (state = initialState, action) => {
  switch (action.type) {
    case USER_CONNECTED:
    case USER_DISCONNECTED:
      return {
        ...state,
        users: action.users,
      };
    default:
      return state;
  }
};
export default reducer;
