import React, { useEffect, useRef } from "react";
import { getPublishJsBundle, getPublishResourceFile } from "../api";
import { runner } from "../core/runner";
import { useShowAlertNotification } from "../InfoAlert";
import useWorkspaceId from "../WorkspacePage/hooks/useWorkspaceId";
import styles from "./styles.module.css";

const PlayPage = () => {
  const workspaceId = useWorkspaceId();
  const canvasRef = useRef();
  const { showError, showInfo } = useShowAlertNotification();

  useEffect(() => {
    const loadBundle = () => getPublishJsBundle(workspaceId);
    const textureUriResolver = (url) => getPublishResourceFile(workspaceId, url);
    const gameInstance = runner(canvasRef.current, {
      loadBundle,
      textureUriResolver,
      onError: showError,
      onInfo: showInfo,
    });
    return gameInstance.cleanup;
  }, [workspaceId, showError, showInfo]);

  return (
    <div className={styles.container}>
      <canvas ref={canvasRef} className={styles.canvas} />
    </div>
  );
};

export default PlayPage;
