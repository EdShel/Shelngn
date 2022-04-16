import clsx from "clsx";
import React, { useCallback, useContext, useState } from "react";
import crossIcon from "../assets/cross.svg";
import styles from "./styles.module.css";

const ShowNotificationContext = React.createContext();

export const InfoAlertProvider = ({ children }) => {
  const [notifications, setNotifications] = useState([]);

  const showNotification = useCallback(
    (notification) => setNotifications((oldState) => [...oldState, notification]),
    []
  );

  const topMostNotification = notifications[0];

  const handleClose = () => {
    setNotifications((oldNotifications) => oldNotifications.slice(1));
  };

  return (
    <ShowNotificationContext.Provider value={showNotification}>
      {children}
      {!!topMostNotification && (
        <div className={clsx(styles.alert, topMostNotification.type === "error" && styles.error)}>
          {topMostNotification.text}
          <button onClick={handleClose} className={styles.close}>
            <img src={crossIcon} alt="Close" />
          </button>
        </div>
      )}
    </ShowNotificationContext.Provider>
  );
};

export const useShowAlertNotification = () => {
  const showAlert = useContext(ShowNotificationContext);
  return {
    showError: (text) => showAlert({ type: "error", text }),
    showInfo: (text) => showAlert({ type: "info", text }),
  };
};
