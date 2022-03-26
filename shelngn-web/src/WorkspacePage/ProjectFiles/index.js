import React, { useState } from "react";
import { useSelector } from "react-redux";
import ContextMenu from "../../components/ContextMenu";
import FilesTree from "../../components/FilesTree";
import { getProjectFiles } from "../selectors";
import { useWorkspaceDispatch } from "../WorkspaceContext";

const ProjectFiles = () => {
  const projectFiles = useSelector(getProjectFiles);
  const [contextMenu, setContextMenu] = useState(null);
  const { workspaceSend } = useWorkspaceDispatch();

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
    const uploadUrl =
      "https://localhost:15555/0dhDWMjAh02BxkIO1cbySg/someFolder/kitten.jpg?sign=IxhaH0YNt4i_c_UpyN-UwU6LLRTbVsgWMdq-OTWpN0c=";
    /** @type File */
    let file;
    for (file of ev.dataTransfer.files) {
      console.log("file", file);
      const response = await fetch(uploadUrl, {
        method: "POST",
        headers: {
          "Content-Range": `bytes 0-${file.size - 1}/${file.size}`,
          "Content-Type": file.type,
        },
        body: file,
      });
      console.log("response", response);
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
