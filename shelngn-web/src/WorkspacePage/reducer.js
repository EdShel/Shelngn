const USER_CONNECTED = "workspace/userConnected";
const USER_DISCONNECTED = "workspace/userDisconnected";

const GAME_PROJECT_INFO = "workspace/gameProject";
const GAME_PROJECT_RENAMED = "workspace/gameProject/rename";
const LIST_PROJECT_FILES = "workspace/ls";

const initialState = {
  users: [],
  project: {
    id: null,
    projectName: null,
  },
  projectFiles: null,
};

const reducer = (state = initialState, action) => {
  switch (action.type) {
    case USER_CONNECTED:
    case USER_DISCONNECTED:
      return {
        ...state,
        users: action.users,
      };
    case GAME_PROJECT_INFO:
      return {
        ...state,
        project: action.gameProject,
      };
    case GAME_PROJECT_RENAMED:
      return {
        ...state,
        project: {
          ...state.project,
          projectName: action.newProjectName,
        },
      };
    case LIST_PROJECT_FILES: {
      return {
        ...state,
        projectFiles: action.projectFiles,
      };
    }
    default:
      return state;
  }
};
export default reducer;
