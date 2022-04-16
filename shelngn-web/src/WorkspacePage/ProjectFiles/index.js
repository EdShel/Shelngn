import React, { useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { postFileUploadRequest } from "../../api";
import ContextMenu from "../../components/ContextMenu";
import FilesTree from "../../components/FilesTree";
import { useShowAlertNotification } from "../../InfoAlert";
import useWorkspaceId from "../hooks/useWorkspaceId";
import { openFile } from "../reducer";
import { getProjectFiles } from "../selectors";
import { useWorkspaceDispatch } from "../WorkspaceContext";
import styles from "./styles.module.css";

const ProjectFiles = ({ className }) => {
  const workspaceId = useWorkspaceId();
  const { workspaceSend, workspaceInvoke } = useWorkspaceDispatch();
  const projectFiles = useSelector(getProjectFiles);
  const [contextMenu, setContextMenu] = useState(null);
  const { showError } = useShowAlertNotification();
  const dispatch = useDispatch();

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
    if (e.folder) {
      items = [
        {
          text: "Create object",
          onClick: async () => {
            const fileName = prompt("Enter file name");
            if (!fileName) {
              return;
            }
            await workspaceSend("createFile", e.folder.id, fileName + ".sobj");
            await workspaceSend("dumpFile", e.folder.id === "." ? fileName : `${e.folder.id}/${fileName}.sobj`, "Test");
          },
        },
        {
          text: "Create folder",
          onClick: () => {
            const name = prompt("Enter folder name");
            workspaceSend("createFolder", e.folder.id, name);
          },
        },
        e.folder !== projectFiles && {
          text: "Delete folder",
          onClick: async () => {
            try {
              await workspaceInvoke("deleteFolder", e.folder.id);
            } catch (e) {
              showError(e.message);
            }
          },
        },
      ].filter((i) => i);
    }
    setContextMenu({
      x: e.clientX,
      y: e.clientY,
      items,
    });
  };

  const handleDrop = async (ev, folder) => {
    for (const droppedItem of ev.dataTransfer.items) {
      if (droppedItem.type === "application/x-fileid") {
        droppedItem.getAsString((fileId) => {
          workspaceSend("moveFile", fileId, folder.id);
        });
      }
      if (droppedItem.type === "application/x-folderid") {
        droppedItem.getAsString((droppedFolderId) => {
          workspaceSend("moveFolder", droppedFolderId, folder.id);
        });
      }

      if (droppedItem.kind === "file") {
        const file = droppedItem.getAsFile();
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
    }
  };

  const handleOpenFile = (file) => dispatch(openFile(file.id, file.name));

  return (
    <div className={styles["project-files"]}>
      {projectFiles && (
        <FilesTree
          className={styles["files-tree"]}
          root={projectFiles}
          onContextMenu={handleContextMenu}
          onDrop={handleDrop}
          onOpenFile={handleOpenFile}
        />
      )}
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
