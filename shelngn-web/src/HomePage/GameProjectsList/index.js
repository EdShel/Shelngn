import React from "react";
import clsx from "clsx";
import { Link } from "react-router-dom";
import UrlTo from "../../UrlTo";
import { ReactComponent as GearsIcon } from "../../assets/gears.svg";
import styles from "./styles.module.css";

const GameProjectsList = ({ gameProjects, className }) => {
  return (
    <div className={clsx(styles["game-projects-list"], className)}>
      {gameProjects?.map((gameProject) => (
        <Link key={gameProject.id} to={UrlTo.workspace(gameProject.id)} className={styles["game-project"]}>
          <p className={styles.title}>{gameProject.projectName}</p>
          <Link to={UrlTo.options(gameProject.id)}>
            <GearsIcon fill="white" className={styles.gearsIcon} />
          </Link>
        </Link>
      ))}
    </div>
  );
};

export default GameProjectsList;
