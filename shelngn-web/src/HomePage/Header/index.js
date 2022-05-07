import React from "react";
import Button from "../../components/Button";
import { Link } from "react-router-dom";
import UrlTo from "../../UrlTo";
import styles from "./styles.module.css";
import AppStorage from "../../AppStorage";
import { useDispatch } from "react-redux";
import { revokeRefreshToken } from "../../auth/reducer";
import clsx from "clsx";

const Header = ({ className }) => {
  const dispatch = useDispatch();

  const handleSignOut = () => {
    dispatch(revokeRefreshToken());
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
