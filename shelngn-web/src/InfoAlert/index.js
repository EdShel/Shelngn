import clsx from "clsx";
import React, { useCallback, useContext, useEffect, useMemo, useState } from "react";
import crossIcon from "../assets/cross.svg";
import LineLoader from "../components/LineLoader";
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
      showError: (text, { autoClose = false } = {}) => showNotification({ type: "error", text, autoClose }),
      showInfo: (text, { autoClose = true } = {}) => showNotification({ type: "info", text, autoClose }),
    }),
    [showNotification]
  );

  const topMostNotification = notifications[0];

  const handleClose = useCallback(() => {
    setNotifications((oldNotifications) => oldNotifications.slice(1));
  }, [setNotifications]);

  useEffect(() => {
    if (!topMostNotification || !topMostNotification.autoClose) {
      return () => {};
    }

    let closeTimeOut = setTimeout(() => {
      if (closeTimeOut) {
        handleClose();
        closeTimeOut = null;
      }
    }, 3000);

    return () => {
      if (closeTimeOut) {
        clearTimeout(closeTimeOut);
      }
    };
  }, [topMostNotification, handleClose]);

  return (
    <ShowNotificationContext.Provider value={showNotificationsShortcuts}>
      {children}
      {!!topMostNotification && (
        <div className={clsx(styles.alert, topMostNotification.type === "error" && styles.error)}>
          {topMostNotification.autoClose && (
            <div className={styles.loading}>
              <LineLoader loadTimeMilliseconds={2500} isLoading />
            </div>
          )}
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
