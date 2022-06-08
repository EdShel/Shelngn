import React, { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import useWorkspaceId from "../../hooks/useWorkspaceId";
import { getCurrentFileId, getOpenFiles, getProjectBuildError, getProjectBuildProgress } from "../../selectors";
import { useWorkspaceDispatch } from "../../WorkspaceContext";
import { buildErrorShown, dumpFile } from "../../reducer";
import { useShowAlertNotification } from "../../../InfoAlert";
import { ReactComponent as PlayIcon } from "../../../assets/play.svg";
import UrlTo from "../../../UrlTo";
import styles from "./styles.module.css";
import Hint from "../../../components/Hint";
import { useTranslation } from "react-i18next";

const RunButtons = () => {
  const workspaceId = useWorkspaceId();
  const { workspaceInvoke } = useWorkspaceDispatch();
  const { showError } = useShowAlertNotification();
  const currentFileId = useSelector(getCurrentFileId);
  const openFiles = useSelector(getOpenFiles);
  const progress = useSelector(getProjectBuildProgress);
  const error = useSelector(getProjectBuildError);
  const {t} = useTranslation();
  const dispatch = useDispatch();

  useEffect(() => {
    if (error) {
      dispatch(buildErrorShown());
      showError(error);
    }
  }, [error]);

  const handleRun = async () => {
    if (currentFileId && openFiles[currentFileId].dirty) {
      await workspaceInvoke("dumpFile", currentFileId, openFiles[currentFileId].content);
      dispatch(dumpFile(currentFileId));
    }

    const buildResult = await workspaceInvoke("build");
    console.log("buildResult", buildResult);
    if (buildResult.shouldOpenProject) {
      window.open(UrlTo.debug(workspaceId)).focus();
    }
  };

  return (
    <Hint className={styles.buttons} renderContent={() => <p className={styles.hint}>{t('workspace.run')}</p>}>
      <button onClick={handleRun} disabled={progress} className={styles.run}>
        <PlayIcon />
      </button>
    </Hint>
  );
};

export default RunButtons;
