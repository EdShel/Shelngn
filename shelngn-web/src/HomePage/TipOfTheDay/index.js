import React from "react";
import { useTranslation } from "react-i18next";
import pixelArtImage from "../assets/pixelArt.webp";
import styles from "./styles.module.css";

const TipOfTheDay = () => {
  const { t } = useTranslation();

  return (
    <section className={styles.container} style={{ backgroundImage: `url(${pixelArtImage})` }}>
      <h2>{t("home.tipOfTheDay")}</h2>
      <p dangerouslySetInnerHTML={{ __html: t("home.tip") }} />
    </section>
  );
};

export default TipOfTheDay;
