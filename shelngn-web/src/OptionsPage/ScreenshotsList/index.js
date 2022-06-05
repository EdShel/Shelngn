import React from "react";
import { getScreenshotUrl } from "../../api";
import useWorkspaceId from "../../WorkspacePage/hooks/useWorkspaceId";
import crossIcon from "../../assets/cross.svg";
import styles from "./styles.module.css";
import { useTranslation } from "react-i18next";

const ScreenshotsList = ({ screenshots, onDeleteScreenshot }) => {
  const workspaceId = useWorkspaceId();
  const { t } = useTranslation();

  return (
    <div className={styles.container}>
      <h2>
        {t("options.screenshots", { count: screenshots.length })} ({screenshots.length})
      </h2>

      {screenshots.length === 0 && (
        <p className={styles.placeholder} dangerouslySetInnerHTML={{ __html: t("options.noScreenshots") }} />
      )}

      <div className={styles.list}>
        {screenshots.map((s) => (
          <div key={s.id} className={styles.screenshot}>
            <img
              className={styles["screenshot-image"]}
              key={s.id}
              alt={t('options.screenshot')}
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
