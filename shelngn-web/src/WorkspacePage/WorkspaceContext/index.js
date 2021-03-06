import React, { useContext, useEffect, useState } from "react";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { useDispatch } from "react-redux";
import { apiUrl } from "../../constants";
import AppStorage from "../../AppStorage";
import { postRefresh } from "../../api";
import useWorkspaceId from '../hooks/useWorkspaceId';
import { resetWorkspace } from "../reducer";

const WorkspaceContext = React.createContext();

export const WorkspaceContextProvider = ({ children }) => {
  const id = useWorkspaceId();
  const dispatch = useDispatch();

  const [workspaceDispatch, setWorkspaceDispatch] = useState({
    workspaceSend: null,
    workspaceInvoke: null,
  });

  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl(`${apiUrl}/workspace?w=${id}`, {
        accessTokenFactory: () => AppStorage.accessToken,
      })
      .build();

    connection.on("redux", (action) => {
      dispatch(action);
    });

    setWorkspaceDispatch({
      workspaceSend: (method, ...args) => connection.send(method, ...args),
      workspaceInvoke: (method, ...args) => connection.invoke(method, ...args),
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

    const init = async () => {
      await connect();

      dispatch(resetWorkspace());
      connection.send("gameProject");
      connection.send("ls");
    };
    init();

    return () => connection.stop();
  }, []);

  return <WorkspaceContext.Provider value={workspaceDispatch}>{children}</WorkspaceContext.Provider>;
};

export const useWorkspaceDispatch = () => {
  return useContext(WorkspaceContext);
};
