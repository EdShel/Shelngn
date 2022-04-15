export const getWorkspaceUsers = (state) => state.workspace.users;

export const getProjectName = (state) => state.workspace.project.projectName;
export const getProjectFiles = (state) => state.workspace.projectFiles;

export const getProjectBuildProgress = (state) => state.workspace.build.progress;
export const getProjectBuildError = (state) => state.workspace.build.error;
