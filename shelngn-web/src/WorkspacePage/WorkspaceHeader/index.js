import React from "react";
import ActiveUsersList from "./ActiveUsersList";
import styles from "./styles.module.css";

const WorkspaceHeader = () => {
  return (
    <nav className={styles.nav}>
      <ActiveUsersList />
    </nav>
  );
};

export default WorkspaceHeader;
