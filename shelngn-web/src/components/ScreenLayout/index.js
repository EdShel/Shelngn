import React from "react";
import SideBar from "../SideBar";
import styles from './styles.module.css';

export const headerClassName = styles.header;
export const contentClassName = styles.content;

const ScreenLayout = ({ children }) => {
  return (
    <div className={styles.container}>
      <SideBar className={styles['side-bar']} />
      {children}
    </div>
  );
};

export default ScreenLayout;
