import React, { useEffect } from "react";
import Header from "./Header";
import Button from "../components/Button";
import ScreenLayout, { headerClassName, contentClassName } from "../components/ScreenLayout";
import Scrollable from "../components/Scrollable";
import { postCreateNewProject } from "../api";
import { useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { getMyGameProjects } from "./selectors";
import UrlTo from "../UrlTo";
import AppStorage from "../AppStorage";
import { loadMyGameProjects } from "./reducer";
import GameProjectsList from "./GameProjectsList";
import pixelArtImage from "./assets/pixelArt.webp";
import styles from "./styles.module.css";

const HomePage = () => {
  const navigate = useNavigate();
  const myGameProjects = useSelector(getMyGameProjects);
  const dispatch = useDispatch();

  const isAuthenticated = !!AppStorage.accessToken;

  useEffect(() => {
    if (!isAuthenticated) {
      return;
    }
    if (myGameProjects.loading) {
      return;
    }
    dispatch(loadMyGameProjects());
  }, [isAuthenticated]);

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
        <h1>Shelngn</h1>
        <h2>Collaborative gameplay prototyping tool</h2>
        {isAuthenticated && <Button text="Create new project for free" onPress={handleCreateProject} />}

        <img src={pixelArtImage} alt="Adventure" className={styles.picture} />

        <GameProjectsList gameProjects={myGameProjects.data} className={styles["game-projects-list"]} />
      </div>
      </Scrollable>
    </ScreenLayout>
  );
};

export default HomePage;
