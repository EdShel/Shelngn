import clsx from "clsx";
import React from "react";
import crossIcon from "../../assets/cross.svg";
import styles from "./styles.module.css";

const TabsPanel = ({ tabs, currentTabId, onChangeCurrentTabId, onCloseTab, children }) => {
  return (
    <div className={styles.container}>
      <div className={styles["tabs-list"]}>
        {(tabs || []).map(({ name, id }) => (
          <div
            key={id}
            className={clsx(styles.tab, id === currentTabId && styles["tab-active"])}
            onClick={() => onChangeCurrentTabId(id)}
            tabIndex={0}
          >
            <span>{name}</span>
            <img
              src={crossIcon}
              alt="Close"
              onClick={(e) => {
                e.stopPropagation();
                onCloseTab(id);
              }}
            />
          </div>
        ))}
      </div>
      <div className={styles["tab-content"]}>{children}</div>
    </div>
  );
};

export default TabsPanel;
