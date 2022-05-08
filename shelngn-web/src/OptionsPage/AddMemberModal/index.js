import React, { useState } from "react";
import InputField from "../../components/InputField";
import LineLoader from "../../components/LineLoader";
import Modal from "../../components/Modal";
import styles from "./styles.module.css";

const AddMemberModal = ({ onAddMember, onCancel }) => {
  const [member, setMember] = useState("");
  const [loading, setLoading] = useState(false);

  const handleAddMember = async () => {
    setLoading(true);
    await onAddMember({ emailOrUserName: member });
    setLoading(false);
  };

  return (
    <Modal onClose={onCancel}>
      <LineLoader isLoading={loading} />
      <Modal.Title text="Add member" />
      <InputField
        labelText="User name or email"
        onChange={(e) => setMember(e.target.value)}
        value={member}
        className={styles["member-text"]}
        autoFocus
      />
      <Modal.OkCancelButtons onOk={handleAddMember} onCancel={onCancel} />
    </Modal>
  );
};

export default AddMemberModal;
