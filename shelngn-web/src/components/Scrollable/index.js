import clsx from "clsx";
import React from "react";
import styles from "./styles.module.css";

const Scrollable = ({ children, className }) => {
  return <div className={clsx(styles.scrollable, className)}>{children}</div>;
};

export default Scrollable;
