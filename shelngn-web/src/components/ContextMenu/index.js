import React, { useEffect, useRef } from "react";
import styles from "./styles.module.css";

const ContextMenu = ({ children, position, onDismiss }) => {
  const elRef = useRef(null);

  useEffect(() => {
    // mostly to make unque reference in case function is reused
    const wrappedDismiss = (e) => {
      e.preventDefault();
      e.stopPropagation();

      onDismiss();
    };

    document.addEventListener("click", wrappedDismiss);
    document.addEventListener("scroll", wrappedDismiss);
    document.addEventListener("contextmenu", wrappedDismiss);

    return () => {
      document.removeEventListener("click", wrappedDismiss);
      document.removeEventListener("scroll", wrappedDismiss);
      document.removeEventListener("contextmenu", wrappedDismiss);
    };
  }, [onDismiss]);

  return (
    <div
      ref={elRef}
      className={styles["context-menu"]}
      style={{
        left: position.x,
        top: position.y,
      }}
    >
      {children}
    </div>
  );
};
export default ContextMenu;

const Item = ({ text, onClick }) => <div className={styles.item} onClick={onClick}>{text}</div>;
ContextMenu.Item = Item;
