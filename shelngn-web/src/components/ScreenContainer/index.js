import React from "react";
import clsx from "clsx";
import styles from "./styles.module.css";

const ScreenContainer = ({ children, className }) => {
  return <main className={clsx(styles.screen, className)}>{children}</main>;
};

export default ScreenContainer;
