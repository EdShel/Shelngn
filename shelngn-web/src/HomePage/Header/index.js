import React from "react";
import Button from "../../components/Button";
import { Link } from "react-router-dom";
import UrlTo from "../../UrlTo";
import styles from "./styles.module.css";
import AppStorage from "../../AppStorage";
import { useDispatch } from "react-redux";
import { revokeRefreshToken } from "../../auth/reducer";

const Header = () => {
  const dispatch = useDispatch();

  const handleSignOut = () => {
    dispatch(revokeRefreshToken());
  };

  const isAuthenticated = !!AppStorage.accessToken;

  return (
    <nav className={styles.header}>
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
