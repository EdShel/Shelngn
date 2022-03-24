import React, { useState } from "react";
import { useSelector } from "react-redux";
import ContextMenu from "../../components/ContextMenu";
import FilesTree from "../../components/FilesTree";
import { getProjectFiles } from "../selectors";

const ProjectFiles = () => {
  const projectFiles = useSelector(getProjectFiles);
  const [contextMenuItem, setContextMenuItem] = useState(null);

  const handleContextMenu = (e) => {
    e.preventDefault();
    e.stopPropagation();

    setContextMenuItem(e);
    console.log("e", e);
  };

  return (
    <div>
      {projectFiles && <FilesTree root={projectFiles} onContextMenu={handleContextMenu} />}
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
