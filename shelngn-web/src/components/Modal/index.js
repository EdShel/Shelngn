import React, { useEffect } from "react";
import { useTranslation } from "react-i18next";
import Button from "../Button";
import styles from "./styles.module.css";

const Modal = ({ onClose, children }) => {
  useEffect(() => {
    const handleKeyPress = (e) => {
      if (e.key === "Escape") {
        onClose();
      }
    };

    document.addEventListener("keydown", handleKeyPress);
    return () => {
      document.removeEventListener("keydown", handleKeyPress);
    };
  });

  return (
    <div className={styles.overlay} onClick={() => onClose?.()}>
      <div className={styles.content} onClick={(e) => e.stopPropagation()}>
        {children}
      </div>
    </div>
  );
};

export default Modal;

Modal.Title = ({ text }) => <h2 className={styles.title}>{text}</h2>;

Modal.Text = ({ text }) => <p className={styles.text}>{text}</p>;

const OkCancelButtons = ({ onOk, onCancel, disabled = false }) => {
  const { t } = useTranslation();
  return (
    <div className={styles["buttons-row"]}>
      <Button type="cancel" text={t("common.cancel")} onPress={onCancel} disabled={disabled} />
      <Button type="primary" text={t("common.ok")} onPress={onOk} disabled={disabled} />
    </div>
  );
};
Modal.OkCancelButtons = OkCancelButtons;
