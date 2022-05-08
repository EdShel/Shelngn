import React from "react";
import clsx from "clsx";
import useUniquedId from "../../hooks/useUniqueId";
import styles from "./styles.module.css";

const InputField = ({
  labelText,
  value,
  name,
  onChange,
  onBlur,
  type = "text",
  className,
  error,
  required,
  autoFocus = false,
}) => {
  const inputId = useUniquedId();

  return (
    <div className={clsx(styles["top-space-keeper"], className)}>
      <div className={styles.container}>
        <input
          name={name}
          id={inputId}
          type={type}
          value={value}
          onChange={onChange}
          onBlur={onBlur}
          className={error ? styles.invalid : null}
          autoFocus={autoFocus}
        />
        <label htmlFor={inputId}>
          {labelText}
          {required && <sup>*</sup>}
        </label>
      </div>
    </div>
  );
};

export default InputField;
