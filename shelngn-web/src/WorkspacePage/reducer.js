const USER_CONNECTED = "workspace/userConnected";
const USER_DISCONNECTED = "workspace/userDisconnected";

const GAME_PROJECT_INFO = "workspace/gameProject";
const GAME_PROJECT_RENAMED = "workspace/gameProject/rename";
const LIST_PROJECT_FILES = "workspace/ls";

const GAME_PROJECT_BUILD_BEGIN = "wokspace/build/begin";
const GAME_PROJECT_BUILD_FINISH = "wokspace/build/finish";
const GAME_PROJECT_BUILD_FAILED = "wokspace/build/failed";
const GAME_PROJECT_BUILD_ERROR_SHOWN = "wokspace/build/errorShown";

const OPEN_FILE = "workspace/openFile";
const CLOSE_FILE = "workspace/closeFile";
const READ_FILE = "workspace/readFile";

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
  files: {}, // fileId: {name: '', content: '', loading: bool, hasContent: bool}
  currentFileId: null,
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
    case OPEN_FILE: {
      const alreadyLoadedFile = state.files[action.fileId];
      return {
        ...state,
        files: {
          ...state.files,
          [action.fileId]: {
            ...alreadyLoadedFile,
            name: action.fileName || alreadyLoadedFile?.name,
            loading: !alreadyLoadedFile,
          },
        },
        currentFileId: action.fileId,
      };
    }
    case CLOSE_FILE:
      const newFiles = { ...state.files };
      delete newFiles[action.fileId];
      return {
        ...state,
        files: newFiles,
        currentFileId: action.fileId === state.currentFileId ? Object.keys(newFiles)[0] : state.currentFileId,
        hasContent: false,
      };
    case READ_FILE:
      if (!state.files[action.fileId]) {
        return state;
      }
      return {
        ...state,
        files: {
          ...state.files,
          [action.fileId]: {
            ...state.files[action.fileId],
            content: action.content,
            hasContent: true,
            loading: false,
          },
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

export const openFile = (fileId, fileName) => ({
  type: OPEN_FILE,
  fileId,
  fileName,
});

export const closeFile = (fileId) => ({
  type: CLOSE_FILE,
  fileId,
});
