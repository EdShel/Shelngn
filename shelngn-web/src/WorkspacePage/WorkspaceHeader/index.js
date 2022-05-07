import clsx from "clsx";
import React from "react";
import ActiveUsersList from "./ActiveUsersList";
import ProjectName from "./ProjectName";
import RunButtons from "./RunButtons";
import styles from "./styles.module.css";

const WorkspaceHeader = ({ className }) => {
  return (
    <nav className={clsx(styles.nav, className)}>
      <ProjectName />
      <RunButtons />
      <ActiveUsersList />
    </nav>
  );
};

export default WorkspaceHeader;
