import clsx from "clsx";
import React, { useRef, useState } from "react";
import { useSelector } from "react-redux";
import { getProjectName } from "../../selectors";
import { useWorkspaceDispatch } from "../../WorkspaceContext";
import Hint from "../../../components/Hint";
import useDebouncedCallback from "../../../hooks/useDebouncedCallback";
import styles from "./styles.module.css";
import { useTranslation } from "react-i18next";

const ProjectName = (className) => {
  const projectName = useSelector(getProjectName);
  const [enteredProjectName, setEnteredProjectName] = useState(null);
  const { workspaceSend } = useWorkspaceDispatch();
  const initialProjectNameRef = useRef();
  const { t } = useTranslation();

  const sendNewProjectName = (newProjectName) => {
    workspaceSend("renameProject", newProjectName);
  };

  const sendNewProjectNameDebounced = useDebouncedCallback(sendNewProjectName, 500);

  const handleFocus = () => {
    initialProjectNameRef.current = projectName;
  };

  const handleBlur = (e) => {
    if (!enteredProjectName) {
      setEnteredProjectName(initialProjectNameRef.current);
      sendNewProjectNameDebounced(initialProjectNameRef.current);
    }
  };

  const handleKeyPress = (e) => {
    if (e.key === "Escape") {
      setEnteredProjectName(initialProjectNameRef.current);
      sendNewProjectNameDebounced(initialProjectNameRef.current);
    }
  };

  const handleChange = (e) => {
    const text = e.target.value;
    if (text) {
      sendNewProjectNameDebounced(text);
    }
    setEnteredProjectName(text);
  };

  return (
    <Hint className={clsx(styles["project-name"], className)} renderContent={() => t("workspace.rename")}>
      <input
        name="projectName"
        onFocus={handleFocus}
        onBlur={handleBlur}
        value={enteredProjectName ?? projectName ?? t("workspace.loading")}
        onKeyDown={handleKeyPress}
        onChange={handleChange}
      />
    </Hint>
  );
};

export default ProjectName;
