import React from "react";
import Button from "../../components/Button";
import Modal from "../../components/Modal";
import styles from "./styles.module.css";

const ConfirmModal = ({ title, text, onOk, onCancel }) => {
  return (
    <Modal onClose={onCancel}>
      <h2 className={styles.title}>{title}</h2>
      <p className={styles.text}>{text}</p>
      <div className={styles["buttons-row"]}>
        <Button type="cancel" text="Cancel" onPress={onCancel} />
        <Button type="primary" text="Ok" onPress={onOk} />
      </div>
    </Modal>
  );
};

export default ConfirmModal;
