import React, { useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { postFileUploadRequest } from "../../api";
import ContextMenu from "../../components/ContextMenu";
import FilesTree from "../../components/FilesTree";
import Scrollable from "../../components/Scrollable";
import { useShowAlertNotification } from "../../InfoAlert";
import useWorkspaceId from "../hooks/useWorkspaceId";
import { openFile } from "../reducer";
import { getProjectFiles } from "../selectors";
import { useWorkspaceDispatch } from "../WorkspaceContext";
import styles from "./styles.module.css";

const wait = (ms) => new Promise((r) => setTimeout(r, ms));

const retry = (operation, delay, retries) =>
  new Promise((resolve, reject) => {
    return operation()
      .then(resolve)
      .catch((reason) => {
        if (retries > 0) {
          return wait(delay)
            .then(retry.bind(null, operation, delay, retries - 1))
            .then(resolve)
            .catch(reject);
        }
        return reject(reason);
      });
  });

async function uploadFileChunked(uploadUrl, file) {
  const chunksSizeBytes = 5 * 1024;
  const chunksCount = Math.ceil(file.size / chunksSizeBytes);
  for (let i = 0; i < chunksCount; i++) {
    const chunkBegin = i * chunksSizeBytes;
    const chunkEnd =  Math.min(chunkBegin + chunksSizeBytes, file.size);

    await retry(
      async () => {
        const response = await fetch(uploadUrl, {
          method: "POST",
          headers: {
            "Content-Range": `bytes 0-${file.size - 1}/${file.size}`,
            "Content-Type": file.type,
          },
          body: file,
        });
        if (!response.ok) {
          throw new Error("Server rejected the file.");
        }
      },
      500,
      5
    );
  }
}

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
          text: "Create script",
          onClick: async () => {
            const fileName = prompt("Enter script name");
            if (!fileName) {
              return;
            }
            const fileNameWithExt = fileName + ".js";
            const fileId = e.folder.id === "." ? fileNameWithExt : `${e.folder.id}/${fileNameWithExt}`;
            await workspaceSend("createFile", e.folder.id, fileNameWithExt);
            // await workspaceSend("dumpFile", e.folder.id === "." ? fileName : `${e.folder.id}/${fileName}.js`, "");
            dispatch(openFile(fileId, fileNameWithExt));
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
    <Scrollable className={styles["project-files"]}>
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
    </Scrollable>
  );
};

export default ProjectFiles;
