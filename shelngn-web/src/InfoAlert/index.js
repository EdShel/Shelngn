import clsx from "clsx";
import React, { useCallback, useContext, useMemo, useState } from "react";
import crossIcon from "../assets/cross.svg";
import styles from "./styles.module.css";

const ShowNotificationContext = React.createContext();

export const InfoAlertProvider = ({ children }) => {
  const [notifications, setNotifications] = useState([]);

  const showNotification = useCallback(
    (notification) => setNotifications((oldState) => [...oldState, notification]),
    []
  );
  const showNotificationsShortcuts = useMemo(
    () => ({
      showError: (text) => showNotification({ type: "error", text }),
      showInfo: (text) => showNotification({ type: "info", text }),
    }),
    [showNotification]
  );

  const topMostNotification = notifications[0];

  const handleClose = () => {
    setNotifications((oldNotifications) => oldNotifications.slice(1));
  };

  return (
    <ShowNotificationContext.Provider value={showNotificationsShortcuts}>
      {children}
      {!!topMostNotification && (
        <div className={clsx(styles.alert, topMostNotification.type === "error" && styles.error)}>
          {notifications.length > 1 && `(1/${notifications.length})`} {topMostNotification.text}
          <button onClick={handleClose} className={styles.close}>
            <img src={crossIcon} alt="Close" />
          </button>
        </div>
      )}
    </ShowNotificationContext.Provider>
  );
};

export const useShowAlertNotification = () => {
  return useContext(ShowNotificationContext);
};
