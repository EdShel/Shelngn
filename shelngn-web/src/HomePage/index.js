import React, { useEffect, useState } from "react";
import Header from "./Header";
import Button from "../components/Button";
import ScreenLayout, { headerClassName, contentClassName } from "../components/ScreenLayout";
import Scrollable from "../components/Scrollable";
import { getHomeGameProjects, getMyGameProjects, postCreateNewProject } from "../api";
import { useNavigate } from "react-router-dom";
import UrlTo from "../UrlTo";
import GameProjectsList from "./GameProjectsList";
import useAuth from "../hooks/useAuth";
import { useShowAlertNotification } from "../InfoAlert";
import LineLoader from "../components/LineLoader";
import styles from "./styles.module.css";
import PublishedGames from "./PublishedGames";
import TipOfTheDay from "./TipOfTheDay";

const HomePage = () => {
  const navigate = useNavigate();
  const [myGameProjects, setMyGameProjects] = useState([]);
  const [otherGameProjects, setOtherGameProjects] = useState(null);
  const [isLoading, setLoading] = useState(true);
  const { isAuthenticated } = useAuth();
  const { showError } = useShowAlertNotification();

  useEffect(() => {
    const loadData = async () => {
      setLoading(true);

      try {
        if (isAuthenticated) {
          setMyGameProjects(await getMyGameProjects());
        }
        setOtherGameProjects(await getHomeGameProjects());
      } catch (e) {
        showError("Error while loading home screen. Please try again later.");
      }

      setLoading(false);
    };

    loadData();
  }, [isAuthenticated, showError]);

  const handleCreateProject = async () => {
    try {
      const { id } = await postCreateNewProject();
      navigate(UrlTo.workspace(id));
    } catch (e) {
      // TOOD: show error toast
    }
  };

  return (
    <ScreenLayout>
      <Header className={headerClassName} />
      <Scrollable className={contentClassName}>
        <div className={styles["main-container"]}>
          <LineLoader isLoading={isLoading} />
          <h1>Shelngn</h1>
          <h2>Collaborative gameplay prototyping tool</h2>
          <TipOfTheDay />

          {isAuthenticated && !!myGameProjects && (
            <GameProjectsList
              gameProjects={myGameProjects.data}
              onCreateProject={handleCreateProject}
              className={styles["game-projects-list"]}
            />
          )}
          {!!otherGameProjects && <PublishedGames games={otherGameProjects.data} />}
        </div>
      </Scrollable>
    </ScreenLayout>
  );
};

export default HomePage;
