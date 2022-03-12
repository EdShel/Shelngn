import React, { useEffect } from "react";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { apiUrl } from "../constants";
import AppStorage from "../AppStorage";
import { useParams } from "react-router-dom";
import { useDispatch } from "react-redux";
import ScreenContainer from "../components/ScreenContainer";
import SideBar from "../components/SideBar";
import WorkspaceHeader from "./WorkspaceHeader";
import styles from "./styles.module.css";
import { postRefresh } from "../api";

const WorkspacePage = () => {
  const { id } = useParams();
  const dispatch = useDispatch();

  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl(`${apiUrl}/workspace?w=${id}`, {
        accessTokenFactory: () => AppStorage.accessToken,
      })
      .build();

    connection.on("redux", (action) => {
      console.log("Workspace action", action);
      dispatch(action);
    });

    const connect = async () => {
      try {
        await connection.start();
      } catch (e) {
        const isUnauthorized = /401/.test(e.message);
        if (isUnauthorized) {
          await postRefresh();
          await connection.start();
        }
      }
    };
    connect();

    return () => connection.stop();
  }, []);

  return (
    <ScreenContainer>
      <SideBar />
      <div className={styles["screen-content"]}>
        <WorkspaceHeader />
      </div>
    </ScreenContainer>
  );
};

export default WorkspacePage;
