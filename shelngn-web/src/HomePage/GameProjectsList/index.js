import React from "react";
import clsx from "clsx";
import styles from "./styles.module.css";
import { Link } from "react-router-dom";
import UrlTo from "../../UrlTo";

const GameProjectsList = ({ gameProjects, className }) => {
  return (
    <div className={clsx(styles["game-projects-list"], className)}>
      {gameProjects?.map((gameProject) => (
        <Link to={UrlTo.workspace(gameProject.id)} className={styles["game-project"]}>
          <p className={styles.title}>{gameProject.projectName}</p>
        </Link>
      ))}
    </div>
  );
};

export default GameProjectsList;
