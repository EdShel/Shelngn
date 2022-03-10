import React, { useEffect } from "react";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { apiUrl } from "../constants";
import AppStorage from "../AppStorage";

const WorkspacePage = () => {
  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl(`${apiUrl}/workspace`, {
        accessTokenFactory: () => AppStorage.accessToken,
      })
      .build();

    connection.on("ReceiveShit", ({ lol }) => console.log(`Received: ${lol}`));

    connection.start();

    return () => connection.stop();
  }, []);

  return <div>WorkspacePage</div>;
};

export default WorkspacePage;
