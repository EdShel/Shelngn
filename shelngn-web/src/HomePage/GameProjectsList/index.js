import React from "react";
import clsx from "clsx";
import { Link, useNavigate } from "react-router-dom";
import UrlTo from "../../UrlTo";
import { ReactComponent as GearsIcon } from "../../assets/gears.svg";
import styles from "./styles.module.css";
import Button from "../../components/Button";
import { useTranslation } from "react-i18next";

const GameProjectsList = ({ gameProjects, onCreateProject, className }) => {
  const navigate = useNavigate();
  const { t } = useTranslation();

  return (
    <section className={clsx(className)} aria-labelledby="gameProjectsList">
      <div className={styles.header}>
        <h2 id="gameProjectsList" className={styles.title}>
          {t("home.yourProjects")} ({gameProjects?.length || 0})
        </h2>
        <Button text={t("home.createNewProject")} onPress={onCreateProject} />
      </div>
      <div className={styles["game-projects-list"]}>
        {gameProjects?.map((gameProject) => (
          <Link key={gameProject.id} to={UrlTo.workspace(gameProject.id)} className={styles["game-project"]}>
            <p className={styles.title}>{gameProject.projectName}</p>
            <GearsIcon
              fill="white"
              className={styles.gearsIcon}
              onClick={(e) => {
                e.preventDefault();
                e.stopPropagation();
                navigate(UrlTo.options(gameProject.id));
              }}
            />
          </Link>
        ))}
      </div>
    </section>
  );
};

export default GameProjectsList;
