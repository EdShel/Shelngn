import React from "react";
import { useSelector } from "react-redux";
import Hint from "../../../components/Hint";
import UserPreview from "../../../components/UserPreview";
import { getWorkspaceUsers } from "../../selectors";
import styles from "./styles.module.css";

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
          <UserPreview userName={user.userName} />
        </Hint>
      ))}
    </div>
  );
};

export default ActiveUsersList;
