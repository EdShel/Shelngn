import React from "react";
import { getScreenshotUrl } from "../../api";
import useWorkspaceId from "../../WorkspacePage/hooks/useWorkspaceId";
import crossIcon from "../../assets/cross.svg";
import styles from "./styles.module.css";

const ScreenshotsList = ({ screenshots, onDeleteScreenshot }) => {
  const workspaceId = useWorkspaceId();

  return (
    <div className={styles.container}>
      <h2>Screenshots ({screenshots.length})</h2>

      {screenshots.length === 0 && (
        <p className={styles.placeholder}>
          There are no screenshots for your game yet. <b>Press F12</b> during debug session to create one.
        </p>
      )}

      <div className={styles.list}>
        {screenshots.map((s) => (
          <div className={styles.screenshot}>
            <img
              className={styles["screenshot-image"]}
              key={s.id}
              alt="Screenshot"
              src={getScreenshotUrl(workspaceId, s.imageUrl)}
            />
            <img
              onClick={() => onDeleteScreenshot(s.id)}
              className={styles["delete-icon"]}
              src={crossIcon}
              alt="Delete screenshot"
            />
          </div>
        ))}
      </div>
    </div>
  );
};

export default ScreenshotsList;
