import React from "react";
import { Helmet } from "react-helmet";
import { useSelector } from "react-redux";
import { getProjectName } from "../selectors";

const PageMeta = () => {
  const projectName = useSelector(getProjectName);

  return (
    <Helmet>
      <title>{projectName}</title>
    </Helmet>
  );
};

export default PageMeta;
