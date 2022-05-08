import React, { useEffect } from "react";
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

Modal.OkCancelButtons = ({ onOk, onCancel, disabled = false }) => (
  <div className={styles["buttons-row"]}>
    <Button type="cancel" text="Cancel" onPress={onCancel} disabled={disabled} />
    <Button type="primary" text="Ok" onPress={onOk} disabled={disabled} />
  </div>
);
