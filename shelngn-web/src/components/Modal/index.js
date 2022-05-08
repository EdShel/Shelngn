import React, { useEffect } from "react";
import styles from "./styles.module.css";

const Modal = ({ onClose, children }) => {
  useEffect(() => {
    const handleKeyPress = (e)=> {
      if (e.key === 'Escape') {
        onClose();
      }
    }
    
    document.addEventListener('keydown', handleKeyPress);
    return () => {
      document.removeEventListener('keydown', handleKeyPress)
    }
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
