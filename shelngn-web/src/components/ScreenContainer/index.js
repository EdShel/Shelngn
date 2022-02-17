import React from "react";
import styles from "./styles.module.css";

const ScreenContainer = ({ children }) => {
  return <main className={styles.screen}>{children}</main>;
};

export default ScreenContainer;
