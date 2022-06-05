import React from "react";
import Button from "../../components/Button";
import { Link } from "react-router-dom";
import UrlTo from "../../UrlTo";
import AppStorage from "../../AppStorage";
import clsx from "clsx";
import { postRevoke } from "../../api";
import styles from "./styles.module.css";
import { useTranslation } from "react-i18next";

const Header = ({ className }) => {
  const { t } = useTranslation();

  const handleSignOut = async () => {
    await postRevoke();
  };

  const isAuthenticated = !!AppStorage.accessToken;

  return (
    <nav className={clsx(styles.header, className)}>
      {isAuthenticated && (
        <Link to={UrlTo.home()} onClick={handleSignOut}>
          <Button text={t("home.signOut")} type="secondary" />
        </Link>
      )}
      {!isAuthenticated && (
        <>
          <Link to={UrlTo.login()}>
            <Button text={t("home.signIn")} type="secondary" />
          </Link>
          <Link to={UrlTo.register()}>
            <Button text={t("home.signUp")} type="primary" />
          </Link>
        </>
      )}
    </nav>
  );
};

export default Header;
