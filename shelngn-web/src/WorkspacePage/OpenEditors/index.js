import React, { useEffect, useMemo, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import CodeEditor from "../../components/CodeEditor";
import TabsPanel from "../../components/TabsPanel";
import useDebouncedCallback from "../../hooks/useDebouncedCallback";
import { closeFile, editFile, openFile } from "../reducer";
import { getCurrentFileId, getOpenFiles } from "../selectors";
import { useWorkspaceDispatch } from "../WorkspaceContext";
import styles from "./styles.module.css";

const OpenEditors = () => {
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
  }, [currentFile]);

  const sendNewCodeDebounced = useDebouncedCallback(
    (fileId, newText) => workspaceSend("dumpFile", fileId, newText),
    2_000
  );
  const handleContentChange = (newText) => {
    dispatch(editFile(currentFileId, newText));
    sendNewCodeDebounced(currentFileId, newText);
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
          value={currentFile.loading ? "Loading..." : currentFile.content}
          onValueChange={handleContentChange}
        />
      )}
    </TabsPanel>
  );
};

export default OpenEditors;
