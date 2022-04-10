import React from "react";
import { getBuiltJsBundle } from "../../../api";
import useWorkspaceId from "../../hooks/useWorkspaceId";

const RunButtons = () => {
  const workspaceId = useWorkspaceId();

  const handleRun = async () => {
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
      <button onClick={handleRun}>Run</button>
    </div>
  );
};

export default RunButtons;
