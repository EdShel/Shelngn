import React, { useEffect } from "react";
import SideBar from "../components/SideBar";
import ScreenContainer from "../components/ScreenContainer";
import Header from "./Header";
import Button from "../components/Button";
import styles from "./styles.module.css";
import { postCreateNewProject } from "../api";
import { useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { getMyGameProjects } from "./selectors";
import UrlTo from "../UrlTo";
import AppStorage from "../AppStorage";
import { loadMyGameProjects } from "./reducer";
import GameProjectsList from "./GameProjectsList";

const HomePage = () => {
  const navigate = useNavigate();
  const myGameProjects = useSelector(getMyGameProjects);
  const dispatch = useDispatch();

  const isAuthenticated = !!AppStorage.accessToken;

  useEffect(() => {
    console.log(1)
    if (!isAuthenticated) {
      return;
    }
    console.log(2)
    if (myGameProjects.loading) {
      return;
    }
    console.log(3)
    dispatch(loadMyGameProjects());
  }, [isAuthenticated]);

  const handleCreateProject = async () => {
    try {
      console.log(4)
      const { id } = await postCreateNewProject();
      console.log('id', id)
      navigate(UrlTo.workspace(id));
    } catch (e) {
      // TOOD: show error toast
    }
  };

  return (
    <ScreenContainer>
      <SideBar />
      <div className={styles["screen-content"]}>
        <Header />
        <div className={styles["main-container"]}>
          <h1>Shelngn</h1>
          <h2>Collaborative gameplay prototyping tool</h2>
          {isAuthenticated && <Button text="Create new project for free" onPress={handleCreateProject} />}

          <GameProjectsList gameProjects={myGameProjects.data} className={styles['game-projects-list']} />
        </div>
      </div>
    </ScreenContainer>
  );
};

export default HomePage;
