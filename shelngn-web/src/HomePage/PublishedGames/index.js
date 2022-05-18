import clsx from "clsx";
import React, { useState } from "react";
import { getScreenshotUrl } from "../../api";
import { ReactComponent as PlayIcon } from "../../assets/play.svg";
import styles from "./styles.module.css";

const PublishedGames = ({ games }) => {
  return (
    <section aria-labelledby="publishedGamesHeader" className={styles.container}>
      <h2 id="publishedGamesHeader">Check out all games</h2>
      <div className={styles["projects-grid"]}>
        {games.map((game) => (
          <Game game={game} />
        ))}
      </div>
    </section>
  );
};

export default PublishedGames;

const Game = ({ game }) => {
  const [currentScreenshot, setCurrentScreenshot] = useState(0);

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
            className={clsx(styles["screenshot"], currentScreenshot !== i && styles.inactive)}
            src={getScreenshotUrl(game.id, pic.imageUrl)}
            alt={game.projectName}
          />
        ))}
        {game.screenshots.length === 0 && <p className={styles["no-screenshots"]}>No screenshots</p>}
        <button className={styles["play-button"]}>
          <PlayIcon />
        </button>
      </div>
      <p className={styles["game-name"]}>{game.projectName}</p>
      <p className={styles.members}>
        Author{!!game.members.length && "s"}:{" "}
        {game.members
          .slice(0, 3)
          .map((m) => m.userName)
          .join(", ")}
      </p>
    </div>
  );
};
