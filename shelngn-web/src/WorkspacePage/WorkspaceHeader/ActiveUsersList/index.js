import React from "react";
import { useSelector } from "react-redux";
import Hint from "../../../components/Hint";
import { getWorkspaceUsers } from "../../selectors";
import styles from "./styles.module.css";

const getUserNameAbbrev = (userName) => userName.match(/\w\w/)?.[0].toUpperCase() || "";

const ActiveUsersList = () => {
  const users = useSelector(getWorkspaceUsers);

  return (
    <div className={styles.list}>
      {users.map((user) => (
        <Hint className={styles.user} renderContent={() => <div>{user.userName}</div>}>
          <span className={styles["user-name"]}>{getUserNameAbbrev(user.userName)}</span>
        </Hint>
      ))}
    </div>
  );
};

export default ActiveUsersList;
