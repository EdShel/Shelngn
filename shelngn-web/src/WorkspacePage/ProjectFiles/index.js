import React, { useState } from "react";
import { useSelector } from "react-redux";
import ContextMenu from "../../components/ContextMenu";
import FilesTree from "../../components/FilesTree";
import { getProjectFiles } from "../selectors";

const ProjectFiles = () => {
  const projectFiles = useSelector(getProjectFiles);
  const [contextMenuItem, setContextMenuItem] = useState(null);

  const handleContextMenu = (e) => {
    if (e.shiftKey) {
      return;
    }
    e.preventDefault();
    e.stopPropagation();

    setContextMenuItem(e);
    console.log("e", e);
  };

  const handleDrop = async (ev, folder) => {
    const uploadUrl =
      "https://localhost:15555/0dhDWMjAh02BxkIO1cbySg/someFolder/kitten.jpg?sign=IxhaH0YNt4i_c_UpyN-UwU6LLRTbVsgWMdq-OTWpN0c=";
    /** @type File */
    let file;
    for (file of ev.dataTransfer.files) {
      console.log('file', file)
      const response = await fetch(uploadUrl, {
        method: 'POST',
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
      {!!contextMenuItem && (
        <ContextMenu
          position={{
            x: contextMenuItem.clientX,
            y: contextMenuItem.clientY,
          }}
          onDismiss={() => setContextMenuItem(null)}
        />
      )}
    </div>
  );
};

export default ProjectFiles;
