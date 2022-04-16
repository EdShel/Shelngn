import React from "react";
import ScreenContainer from "../components/ScreenContainer";
import SideBar from "../components/SideBar";
import WorkspaceHeader from "./WorkspaceHeader";
import PageMeta from "./PageMeta";
import styles from "./styles.module.css";
import { WorkspaceContextProvider } from "./WorkspaceContext";
import ProjectFiles from "./ProjectFiles";
import SplitPane from "../components/SplitPane";
import OpenEditors from "./OpenEditors";

const WorkspacePage = () => {
  return (
    <WorkspaceContextProvider>
      <ScreenContainer className={styles.screen}>
        <PageMeta />
        <SideBar />
        <div className={styles["screen-content"]}>
          <WorkspaceHeader />
          <SplitPane className={styles.split} left={<ProjectFiles />} right={<OpenEditors className={styles.code} />} />
        </div>
      </ScreenContainer>
    </WorkspaceContextProvider>
  );
};

export default WorkspacePage;
