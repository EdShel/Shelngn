import React from "react";
import clsx from "clsx";
import styles from "./styles.module.css";

const Button = ({
  className,
  text,
  onPress,
  type = "primary",
  disabled = false,
}) => {
  return (
    <button
      className={clsx(styles.button, styles[type], className)}
      onClick={onPress}
      disabled={disabled}
    >
      {text}
    </button>
  );
};

export default Button;
