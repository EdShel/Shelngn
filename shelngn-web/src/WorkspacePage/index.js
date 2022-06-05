import React from "react";
import WorkspaceHeader from "./WorkspaceHeader";
import PageMeta from "./PageMeta";
import { WorkspaceContextProvider } from "./WorkspaceContext";
import ProjectFiles from "./ProjectFiles";
import SplitPane from "../components/SplitPane";
import OpenEditors from "./OpenEditors";
import styles from "./styles.module.css";
import ScreenLayout, { contentClassName, headerClassName } from "../components/ScreenLayout";
import clsx from "clsx";

const WorkspacePage = () => {
  return (
    <WorkspaceContextProvider>
      <ScreenLayout>
        <PageMeta />
        <WorkspaceHeader className={headerClassName} />
        <div className={clsx(styles["screen-content"], contentClassName)}>
          <SplitPane className={styles.split} left={<ProjectFiles />} right={<OpenEditors className={styles.code} />} />
        </div>
      </ScreenLayout>
    </WorkspaceContextProvider>
  );
};

export default WorkspacePage;
