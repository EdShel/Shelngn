import React from "react";
import ScreenContainer from "../components/ScreenContainer";
import SideBar from "../components/SideBar";
import WorkspaceHeader from "./WorkspaceHeader";
import PageMeta from "./PageMeta";
import styles from "./styles.module.css";
import { WorkspaceContextProvider } from "./WorkspaceContext";
import ProjectFiles from "./ProjectFiles";

const WorkspacePage = () => {
  return (
    <WorkspaceContextProvider>
      <ScreenContainer>
        <PageMeta />
        <SideBar />
        <div className={styles["screen-content"]}>
          <WorkspaceHeader />
          <ProjectFiles />
        </div>
      </ScreenContainer>
    </WorkspaceContextProvider>
  );
};

export default WorkspacePage;
