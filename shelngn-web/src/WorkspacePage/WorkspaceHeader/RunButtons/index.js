import React, { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { getBuiltJsBundle } from "../../../api";
import useWorkspaceId from "../../hooks/useWorkspaceId";
import { getProjectBuildError, getProjectBuildProgress } from "../../selectors";
import { useWorkspaceDispatch } from "../../WorkspaceContext";
import { buildErrorShown } from "../../reducer";
import { useShowAlertNotification } from "../../../InfoAlert";

const RunButtons = () => {
  const workspaceId = useWorkspaceId();
  const { workspaceInvoke } = useWorkspaceDispatch();
  const { showError } = useShowAlertNotification();
  const progress = useSelector(getProjectBuildProgress);
  const error = useSelector(getProjectBuildError);
  const dispatch = useDispatch();

  useEffect(() => {
    if (error) {
      dispatch(buildErrorShown());
      showError(error);
    }
  }, [error]);

  const handleRun = async () => {
    await workspaceInvoke("build");

    const bundleSourceCode = await getBuiltJsBundle(workspaceId);
    const bundleBlob = new Blob([bundleSourceCode]);
    console.log("bundleBlob", bundleBlob);
    const scriptUrl = URL.createObjectURL(bundleBlob);
    console.log("scriptUrl", scriptUrl);
    const scriptWorker = new Worker(scriptUrl);
    scriptWorker.onmessage = (msg) => {
      console.log("msg", msg);
      msg();
    };
    scriptWorker.postMessage("Hello");
    URL.revokeObjectURL(scriptUrl);

    setTimeout(() => {
      scriptWorker.terminate();
    }, 1000);
  };

  return (
    <div>
      <button onClick={handleRun} disabled={progress}>
        Run
      </button>
    </div>
  );
};

export default RunButtons;
