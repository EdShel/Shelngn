import React from "react";
import { useTranslation } from "react-i18next";
import planetIcon from "../../../assets/planet.svg";
import styles from "./styles.module.css";

const LanguagePicker = () => {
  const { i18n } = useTranslation();

  const handleChangeLanguage = (newLanguage) => {
    i18n.changeLanguage(newLanguage);
    localStorage.setItem("language", newLanguage);
  };

  return (
    <div className={styles.container}>
      <div className={styles.i18n}>
        <img src={planetIcon} alt="Language" />
        <div className={styles.hint}>
          <button className={styles.lang} onClick={() => handleChangeLanguage("en")}>
            {i18n.language === "en" ? <b>English</b> : <span>English</span>}
          </button>
          <button className={styles.lang} onClick={() => handleChangeLanguage("uk")}>
            {i18n.language === "uk" ? <b>Українська</b> : <span>Українська</span>}
          </button>
        </div>
      </div>
    </div>
  );
};

export default LanguagePicker;
