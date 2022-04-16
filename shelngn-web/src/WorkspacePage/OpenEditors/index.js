import React, { useEffect, useMemo, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import CodeEditor from "../../components/CodeEditor";
import TabsPanel from "../../components/TabsPanel";
import useDebouncedCallback from "../../hooks/useDebouncedCallback";
import { closeFile, openFile } from "../reducer";
import { getCurrentFileId, getOpenFiles } from "../selectors";
import { useWorkspaceDispatch } from "../WorkspaceContext";
import styles from "./styles.module.css";

const OpenEditors = () => {
  const [localCodeText, setLocalCodeText] = useState(null);
  const openFiles = useSelector(getOpenFiles);
  const currentFileId = useSelector(getCurrentFileId);
  const dispatch = useDispatch();
  const { workspaceSend } = useWorkspaceDispatch();

  const tabs = useMemo(() => Object.entries(openFiles).map(([id, file]) => ({ id, name: file.name })), [openFiles]);
  const currentFile = openFiles[currentFileId];

  useEffect(() => {
    if (!currentFile) {
      return;
    }
    if (currentFile.loading && !currentFile.hasContent) {
      workspaceSend("readFile", currentFileId);
      return;
    }
    setLocalCodeText(currentFile.content);
  }, [currentFile]);

  const sendNewCodeDebounced = useDebouncedCallback(
    (newText) => workspaceSend("dumpFile", currentFileId, newText),
    500
  );
  const handleContentChange = (newText) => {
    setLocalCodeText(newText);
    sendNewCodeDebounced(newText);
  };

  return (
    <TabsPanel
      tabs={tabs}
      onChangeCurrentTabId={(id) => dispatch(openFile(id))}
      currentTabId={currentFileId}
      onCloseTab={(id) => dispatch(closeFile(id))}
    >
      {!currentFile && <div>Choose a file</div>}
      {currentFile && (
        <CodeEditor
          className={styles.code}
          value={currentFile.loading || localCodeText === null ? "Loading..." : localCodeText}
          onValueChange={handleContentChange}
        />
      )}
    </TabsPanel>
  );
};

export default OpenEditors;
