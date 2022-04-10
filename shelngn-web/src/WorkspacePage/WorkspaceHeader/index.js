import React from "react";
import ActiveUsersList from "./ActiveUsersList";
import ProjectName from "./ProjectName";
import RunButtons from "./RunButtons";
import styles from "./styles.module.css";

const WorkspaceHeader = () => {
  return (
    <nav className={styles.nav}>
      <ProjectName />
      <RunButtons />
      <ActiveUsersList />
    </nav>
  );
};

export default WorkspaceHeader;
