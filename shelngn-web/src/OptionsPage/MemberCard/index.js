import React from "react";
import Hint from "../../components/Hint";
import UserPreview from "../../components/UserPreview";
import crossIcon from "../../assets/cross.svg";
import crownIcon from "../../assets/crown.svg";
import developerIcon from "../../assets/developer.svg";
import styles from "./styles.module.css";
import { useTranslation } from "react-i18next";

const MemberCard = ({ userName, role, email, isMe, canBeDeleted, onRemove }) => {
  const { t } = useTranslation();
  return (
    <div className={styles["user-card"]}>
      <div className={styles["user-image"]}>
        <UserPreview userName={userName} className={styles["user-avatar"]} />
        <Hint renderContent={() => t(`options.roles.${role}`)} className={styles["role-icon"]}>
          <img src={role === "Owner" ? crownIcon : developerIcon} alt={role} />
        </Hint>
      </div>
      <p>{email}</p>
      <p>
        {userName} {isMe && `(${t("options.you")})`}
      </p>
      {canBeDeleted && (
        <Hint renderContent={() => t("options.removeMember")} className={styles["remove-user"]}>
          <img src={crossIcon} alt={t("options.remove")} onClick={onRemove} />
        </Hint>
      )}
    </div>
  );
};

export default MemberCard;
