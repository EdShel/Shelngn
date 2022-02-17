import React from "react";
import clsx from "clsx";
import styles from "./styles.module.css";

const Form = ({ title, children, className, enableAutocomplete }) => {
  return (
    <form
      className={clsx(styles["form-container"], className)}
      autoComplete={enableAutocomplete ? "on" : "off"}
    >
      <div>
        <h1>{title}</h1>
        <fieldset className={styles.form}>{children}</fieldset>
      </div>
    </form>
  );
};

export default Form;
