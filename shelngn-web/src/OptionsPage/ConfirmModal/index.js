import React from "react";
import Modal from "../../components/Modal";

const ConfirmModal = ({ title, text, onOk, onCancel }) => {
  return (
    <Modal onClose={onCancel}>
      <Modal.Title text={title} />
      <Modal.Text text={text} />
      <Modal.OkCancelButtons onOk={onOk} onCancel={onCancel} />
    </Modal>
  );
};

export default ConfirmModal;
