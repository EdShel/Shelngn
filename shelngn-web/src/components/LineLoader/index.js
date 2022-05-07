import clsx from "clsx";
import React from "react";
import styles from "./styles.module.css";

const LineLoader = ({ isLoading }) => {
  return (
    <div className={clsx(styles["line-loader"], isLoading ? styles.animate : styles.hide)}>
      <div className={styles["space-keep"]} />
    </div>
  );
};

export default LineLoader;
