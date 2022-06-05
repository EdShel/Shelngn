import clsx from "clsx";
import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import { getScreenshotUrl } from "../../api";
import { ReactComponent as PlayIcon } from "../../assets/play.svg";
import UrlTo from "../../UrlTo";
import styles from "./styles.module.css";

const PublishedGames = ({ games }) => {
  const { t } = useTranslation();

  return (
    <section aria-labelledby="publishedGamesHeader" className={styles.container}>
      <h2 id="publishedGamesHeader">{t("home.checkOutAllGames")}</h2>
      <div className={styles["projects-grid"]}>
        {games.map((game) => (
          <Game game={game} key={game.id} />
        ))}
      </div>
      {games.length === 0 && <p>{t("home.noPublishedGames")}</p>}
    </section>
  );
};

export default PublishedGames;

const Game = ({ game }) => {
  const [currentScreenshot, setCurrentScreenshot] = useState(0);
  const { t } = useTranslation();

  const handleChangeSlide = () => {
    if (!game.screenshots.length) {
      return;
    }
    setCurrentScreenshot((oldValue) => (oldValue + 1) % game.screenshots.length);
  };

  return (
    <div>
      <div className={styles.screenshots} onClick={handleChangeSlide}>
        {game.screenshots.map((pic, i) => (
          <img
            key={pic.id}
            className={clsx(styles["screenshot"], currentScreenshot !== i && styles.inactive)}
            src={getScreenshotUrl(game.id, pic.imageUrl)}
            alt={game.projectName}
          />
        ))}
        {game.screenshots.length === 0 && <p className={styles["no-screenshots"]}>No screenshots</p>}
        <Link
          to={UrlTo.play(game.id)}
          className={styles["play-button"]}
          onClick={(e) => e.stopPropagation()}
          target="_blank"
        >
          <PlayIcon />
        </Link>
      </div>
      <p className={styles["game-name"]}>{game.projectName}</p>
      <p className={styles.members}>
        {t("home.authors", { count: game.members.length })}:{" "}
        {game.members
          .slice(0, 3)
          .map((m) => m.userName)
          .join(", ")}
      </p>
    </div>
  );
};
