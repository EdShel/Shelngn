import React, { useState } from "react";
import { useSelector } from "react-redux";
import { postFileUploadRequest } from "../../api";
import ContextMenu from "../../components/ContextMenu";
import FilesTree from "../../components/FilesTree";
import { useShowAlertNotification } from "../../InfoAlert";
import useWorkspaceId from "../hooks/useWorkspaceId";
import { getProjectFiles } from "../selectors";
import { useWorkspaceDispatch } from "../WorkspaceContext";

const ProjectFiles = () => {
  const workspaceId = useWorkspaceId();
  const { workspaceSend } = useWorkspaceDispatch();
  const projectFiles = useSelector(getProjectFiles);
  const [contextMenu, setContextMenu] = useState(null);
  const { showError } = useShowAlertNotification();

  const handleContextMenu = (e) => {
    if (e.shiftKey) {
      return;
    }
    e.preventDefault();
    e.stopPropagation();

    let items = [];
    if (e.file) {
      items = [{ text: "Delete file", onClick: () => workspaceSend("deleteFile", e.file.id) }];
    }
    setContextMenu({
      x: e.clientX,
      y: e.clientY,
      items,
    });
    console.log("e", e);
  };

  const handleDrop = async (ev, folder) => {
    /** @type File */
    let file;
    for (file of ev.dataTransfer.files) {
      let uploadUrl = null;
      try {
        const fileName = file.name.split("/").pop();
        const responseData = await postFileUploadRequest(workspaceId, folder.id + "/" + fileName, file.type);
        uploadUrl = responseData.signedUrl;
      } catch (ex) {
        showError("The file name or the file itself are invalid.");
        return;
      }
      try {
        const response = await fetch(uploadUrl, {
          method: "POST",
          headers: {
            "Content-Range": `bytes 0-${file.size - 1}/${file.size}`,
            "Content-Type": file.type,
          },
          body: file,
        });
        if (!response.ok) {
          showError("The server rejected the file.");
          return;
        }
        await workspaceSend("uploadFile");
      } catch (ex) {
        showError("Error while uploading the file.");
      }
    }
  };

  return (
    <div>
      {projectFiles && <FilesTree root={projectFiles} onContextMenu={handleContextMenu} onDrop={handleDrop} />}
      {!!contextMenu && (
        <ContextMenu
          position={{
            x: contextMenu.x,
            y: contextMenu.y,
          }}
          onDismiss={() => setContextMenu(null)}
        >
          {contextMenu.items.map((item) => (
            <ContextMenu.Item key={item.text} text={item.text} onClick={item.onClick} />
          ))}
        </ContextMenu>
      )}
    </div>
  );
};

export default ProjectFiles;
