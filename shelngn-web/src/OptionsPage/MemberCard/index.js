import React from "react";
import Hint from "../../components/Hint";
import UserPreview from "../../components/UserPreview";
import crossIcon from "../../assets/cross.svg";
import crownIcon from "../../assets/crown.svg";
import developerIcon from "../../assets/developer.svg";
import styles from "./styles.module.css";

const MemberCard = ({ userName, role, email, isMe, canBeDeleted, onRemove }) => {
  return (
    <div className={styles["user-card"]}>
      <div className={styles["user-image"]}>
        <UserPreview userName={userName} className={styles["user-avatar"]} />
        <Hint renderContent={() => role} className={styles["role-icon"]}>
          <img src={role === "Owner" ? crownIcon : developerIcon} alt={role} />
        </Hint>
      </div>
      <p>{email}</p>
      <p>
        {userName} {isMe && "(You)"}
      </p>
      {canBeDeleted && (
        <Hint renderContent={() => "Remove member"} className={styles["remove-user"]}>
          <img src={crossIcon} alt="Remove" onClick={onRemove} />
        </Hint>
      )}
    </div>
  );
};

export default MemberCard;
