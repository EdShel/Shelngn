import React, { useMemo } from "react";
import { useDispatch, useSelector } from "react-redux";
import TabsPanel from "../../components/TabsPanel";
import { closeFile, openFile } from "../reducer";
import { getCurrentFileId, getOpenFiles } from "../selectors";
import ChooseFile from "./ChooseFile";
import ImagePreview from "./ImageEditor";
import JsEditor from "./JsEditor";

const OpenEditors = () => {
  const openFiles = useSelector(getOpenFiles);
  const currentFileId = useSelector(getCurrentFileId);
  const dispatch = useDispatch();

  const tabs = useMemo(() => Object.entries(openFiles).map(([id, file]) => ({ id, name: file.name })), [openFiles]);
  const currentFile = openFiles[currentFileId];

  const renderCurrentTab = () => {
    const fileExtension = currentFileId.substring(currentFileId.lastIndexOf("."));
    switch (fileExtension) {
      case ".js":
        return <JsEditor currentFileId={currentFileId} currentFile={currentFile} />;
      case ".jpg":
      case ".png":
        return <ImagePreview currentFileId={currentFileId} />;
      default:
        return <div>The file preview is not supported.</div>;
    }
  };

  return (
    <TabsPanel
      tabs={tabs}
      onChangeCurrentTabId={(id) => dispatch(openFile(id))}
      currentTabId={currentFileId}
      onCloseTab={(id) => dispatch(closeFile(id))}
    >
      {!currentFile && <ChooseFile />}
      {currentFile && renderCurrentTab()}
    </TabsPanel>
  );
};

export default OpenEditors;
