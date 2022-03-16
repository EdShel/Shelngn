import React from "react";
import { useSelector } from "react-redux";
import FilesTree from "../../components/FilesTree";
import { getProjectFiles } from "../selectors";

const ProjectFiles = () => {
  const projectFiles = useSelector(getProjectFiles);

  return <div>{projectFiles && <FilesTree root={projectFiles} />}</div>;
};

export default ProjectFiles;
