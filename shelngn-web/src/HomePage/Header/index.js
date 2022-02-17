import React from "react";
import Button from "../../components/Button";
import { Link } from "react-router-dom";
import UrlTo from "../../UrlTo";
import styles from "./styles.module.css";

const Header = () => {
  return (
    <nav className={styles.header}>
      <Link to={UrlTo.login()}>
        <Button text="Sign in" type="secondary" />
      </Link>
      <Link to={UrlTo.register()}>
        <Button text="Sign up" type="primary" />
      </Link>
    </nav>
  );
};

export default Header;
