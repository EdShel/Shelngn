import React, { useEffect } from "react";
import { useDispatch } from "react-redux";
import CodeEditor from "../../../components/CodeEditor";
import useDebouncedCallback from "../../../hooks/useDebouncedCallback";
import { dumpFile, editFile } from "../../reducer";
import { useWorkspaceDispatch } from "../../WorkspaceContext";
import styles from "./styles.module.css";

const JsEditor = ({ currentFile, currentFileId }) => {
  const { workspaceSend } = useWorkspaceDispatch();
  const dispatch = useDispatch();

  useEffect(() => {
    if (!currentFile) {
      return;
    }
    if (currentFile.loading && !currentFile.hasContent) {
      workspaceSend("readFile", currentFileId);
      return;
    }
  }, [currentFile]);

  const sendNewCodeDebounced = useDebouncedCallback((fileId, newText) => {
    workspaceSend("dumpFile", fileId, newText);
    dispatch(dumpFile(fileId));
  }, 2_000);
  const handleContentChange = (newText) => {
    dispatch(editFile(currentFileId, newText));
    sendNewCodeDebounced(currentFileId, newText);
  };

  return (
    <CodeEditor
      className={styles.code}
      value={currentFile.loading ? "Loading..." : currentFile.content}
      onValueChange={handleContentChange}
    />
  );
};

export default JsEditor;
