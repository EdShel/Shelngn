import React from "react";
import Button from "../../components/Button";
import { Link } from "react-router-dom";
import UrlTo from "../../UrlTo";
import AppStorage from "../../AppStorage";
import clsx from "clsx";
import { postRevoke } from "../../api";
import styles from "./styles.module.css";

const Header = ({ className }) => {
  const handleSignOut = async () => {
    await postRevoke();
  };

  const isAuthenticated = !!AppStorage.accessToken;

  return (
    <nav className={clsx(styles.header, className)}>
      {isAuthenticated && (
        <Link to={UrlTo.home()} onClick={handleSignOut}>
          <Button text="Sign out" type="secondary" />
        </Link>
      )}
      {!isAuthenticated && (
        <>
          <Link to={UrlTo.login()}>
            <Button text="Sign in" type="secondary" />
          </Link>
          <Link to={UrlTo.register()}>
            <Button text="Sign up" type="primary" />
          </Link>
        </>
      )}
    </nav>
  );
};

export default Header;
