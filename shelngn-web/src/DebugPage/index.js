import React, { useCallback, useEffect, useRef, useState } from "react";
import useWorkspaceId from "../WorkspacePage/hooks/useWorkspaceId";
import { addFpsChangeListener } from "../core/rendering/Fps";
import { getBuiltJsBundle, getBuiltResourceFile, postFileUploadRequest, postScreenshotUploaded } from "../api";
import styles from "./styles.module.css";
import { useShowAlertNotification } from "../InfoAlert";
import { runner } from "../core/runner";
import useScreenshot from "../hooks/useScreenshot";

const DebugPage = () => {
  const workspaceId = useWorkspaceId();
  const canvasRef = useRef();
  const { showError, showInfo } = useShowAlertNotification();
  const [fps, setFps] = useState(0);

  const handleScreenshot = useCallback(
    async (screenshotBlob) => {
      let uploadUrl = null;
      let filePath;
      try {
        const fileName = new Date().toISOString().replace(/[-:Z.]/g, "") + ".png";
        filePath = `screenshots/${fileName}`;
        const responseData = await postFileUploadRequest(workspaceId, filePath, "image/png");
        uploadUrl = responseData.signedUrl;
      } catch (ex) {
        showError("The screenshot name or the image itself are invalid.");
        return;
      }
      try {
        const response = await fetch(uploadUrl, {
          method: "POST",
          headers: {
            "Content-Range": `bytes 0-${screenshotBlob.size - 1}/${screenshotBlob.size}`,
            "Content-Type": screenshotBlob.type,
          },
          body: screenshotBlob,
        });
        if (!response.ok) {
          showError("The server rejected the file.");
          return;
        }
        await postScreenshotUploaded(workspaceId, filePath);
        showInfo("Saved screenshot", { autoClose: true });
      } catch (ex) {
        showError("Error while uploading the screenshot.");
      }
    },
    [workspaceId, showError, showInfo]
  );

  useScreenshot({ onScreenshot: handleScreenshot, canvas: canvasRef.current });

  useEffect(() => {
    const loadBundle = () => getBuiltJsBundle(workspaceId);
    const textureUriResolver = (url) => getBuiltResourceFile(workspaceId, url);
    const gameInstance = runner(canvasRef.current, {
      loadBundle,
      textureUriResolver,
      onError: showError,
      onInfo: showInfo,
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
