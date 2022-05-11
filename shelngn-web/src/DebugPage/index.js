import React, { useEffect, useRef, useState } from "react";
import useWorkspaceId from "../WorkspacePage/hooks/useWorkspaceId";
import { addFpsChangeListener } from "../core/rendering/Fps";
import { getBuiltJsBundle, getBuiltResourceFile } from "../api";
import styles from "./styles.module.css";
import { useShowAlertNotification } from "../InfoAlert";
import { runner } from "../core/runner";

const DebugPage = () => {
  const workspaceId = useWorkspaceId();
  const [fps, setFps] = useState(0);
  const canvasRef = useRef();
  const { showError } = useShowAlertNotification();

  useEffect(() => {
    const loadBundle = () => getBuiltJsBundle(workspaceId);
    const textureUriResolver = (url) => getBuiltResourceFile(workspaceId, url);
    const onError = (e) => showError(e);
    const gameInstance = runner(canvasRef.current, {
      loadBundle,
      textureUriResolver,
      onError,
    });
    return gameInstance.cleanup;
  }, [workspaceId]);

  useEffect(() => {
    const subscriber = addFpsChangeListener(setFps);
    return () => subscriber.remove();
  }, []);

  return (
    <div className={styles.container}>
      <canvas ref={canvasRef} className={styles.canvas} />
      <div className={styles["fps-counter"]}>{fps} FPS</div>
    </div>
  );
};

export default DebugPage;
