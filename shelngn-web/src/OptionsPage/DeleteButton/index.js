import React from "react";
import { useTranslation } from "react-i18next";
import styles from "./styles.module.css";

const DeleteButton = ({ onPress }) => {
  const { t } = useTranslation();
  return (
    <div className={styles.container} onClick={onPress}>
      {t("options.delete")}
    </div>
  );
};

export default DeleteButton;
