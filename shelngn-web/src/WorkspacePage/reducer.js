const USER_CONNECTED = "workspace/userConnected";
const USER_DISCONNECTED = "workspace/userDisconnected";

const GAME_PROJECT_INFO = "workspace/gameProject";
const GAME_PROJECT_RENAMED = "workspace/gameProject/rename";
const LIST_PROJECT_FILES = "workspace/ls";

const GAME_PROJECT_BUILD_BEGIN = "wokspace/build/begin";
const GAME_PROJECT_BUILD_FINISH = "wokspace/build/finish";
const GAME_PROJECT_BUILD_FAILED = "wokspace/build/failed";
const GAME_PROJECT_BUILD_ERROR_SHOWN = "wokspace/build/errorShown";

const initialState = {
  users: [],
  project: {
    id: null,
    projectName: null,
  },
  projectFiles: null,
  build: {
    progress: false,
    error: null,
  },
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
    case GAME_PROJECT_BUILD_BEGIN:
      return {
        ...state,
        build: {
          progress: true,
          error: null,
        },
      };
    case GAME_PROJECT_BUILD_FINISH:
      return {
        ...state,
        build: {
          ...state.build,
          progress: false,
        },
      };
    case GAME_PROJECT_BUILD_FAILED:
      return {
        ...state,
        build: {
          progress: false,
          error: action.error,
        },
      };
    case GAME_PROJECT_BUILD_ERROR_SHOWN:
      return {
        ...state,
        build: {
          ...state.build,
          error: null,
        },
      };
    default:
      return state;
  }
};
export default reducer;

export const buildErrorShown = () => ({
  type: GAME_PROJECT_BUILD_ERROR_SHOWN,
});
