import React from "react";
import SideBar from "../components/SideBar";
import ScreenContainer from "../components/ScreenContainer";
import Header from "./Header";
import styles from "./styles.module.css";

const HomePage = () => {
  return (
    <ScreenContainer>
      <SideBar />
      <div className={styles['screen-content']}>
        <Header />
        <div>
          <h1>Shelngn</h1>
          <h2>Collaborative gameplay prototyping tool</h2>
        </div>
      </div>
    </ScreenContainer>
  );
};

export default HomePage;
