import React from "react";
import { useSelector } from "react-redux";
import Hint from "../../../components/Hint";
import { getWorkspaceUsers } from "../../selectors";
import styles from "./styles.module.css";

const getUserNameAbbrev = (userName) => userName.match(/\w\w/)?.[0].toUpperCase() || "";

const ActiveUsersList = () => {
  const users = useSelector(getWorkspaceUsers);

  if (!users.length) {
    return null;
  }

  return (
    <div className={styles.list}>
      {users.map((user) => (
        <Hint
          key={user.connectionId}
          className={styles["user-container"]}
          renderContent={() => <div>{user.userName}</div>}
        >
          <div className={styles.user}>
            <span className={styles["user-name"]}>{getUserNameAbbrev(user.userName)}</span>
          </div>
        </Hint>
      ))}
    </div>
  );
};

export default ActiveUsersList;
