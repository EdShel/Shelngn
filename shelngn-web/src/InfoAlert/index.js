import React, { useCallback, useContext, useState } from "react";
import styles from "./styles.module.css";

const ShowNotificationContext = React.createContext();

export const InfoAlertProvider = ({ children }) => {
  const [notifications, setNotifications] = useState([]);

  const showNotification = useCallback(
    (notification) => setNotifications((oldState) => [...oldState, notification]),
    []
  );

  const topMostNotification = notifications[0];

  return (
    <ShowNotificationContext.Provider value={showNotification}>
      {children}
      {!!topMostNotification && <div className={styles.alert}>{topMostNotification.text}</div>}
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

