import clsx from "clsx";
import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { deleteMember, deleteProject, getProjectInfo } from "../api";
import LineLoader from "../components/LineLoader";
import Modal from "../components/Modal";
import ScreenLayout, { contentClassName } from "../components/ScreenLayout";
import UserPreview from "../components/UserPreview";
import UrlTo from "../UrlTo";
import useWorkspaceId from "../WorkspacePage/hooks/useWorkspaceId";
import ConfirmModal from "./ConfirmModal";
import DeleteButton from "./DeleteButton";
import styles from "./styles.module.css";

const OptionsPage = () => {
  const workspaceId = useWorkspaceId();
  const [project, setProject] = useState(null);
  const [isLoading, setLoading] = useState(true);
  const [showDeleteProject, setShowDeleteProject] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchProject = async () => {
      const projectResponse = await getProjectInfo(workspaceId);
      setProject(projectResponse);
      setLoading(false);
    };
    fetchProject();
  }, [workspaceId]);

  return (
    <ScreenLayout>
      <LineLoader isLoading={isLoading} />
      <div className={clsx(contentClassName, styles.container)}>
        {project && (
          <>
            <div className={styles.header}>
              <h1 className={styles["header-text"]}>{project.projectName}</h1>
              <p>Created {new Date(project.insertDate).toLocaleString("en")}</p>
              <DeleteButton onPress={() => setShowDeleteProject(true)} />
            </div>
            <h2>Members ({project.members.length})</h2>
            <div className={styles.members}>
              {project.members.map((member) => (
                <div className={styles["user-card"]}>
                  <UserPreview userName={member.userName} className={styles["user-avatar"]} />
                  <p>{member.email}</p>
                  <p>{member.userName}</p>
                </div>
              ))}
            </div>
          </>
        )}
      </div>
      {showDeleteProject && (
        <ConfirmModal
          title="Delete project"
          text="Do you really want to delete the project?"
          onCancel={() => setShowDeleteProject(false)}
          onOk={async () => {
            try {
              await deleteProject(workspaceId);
              navigate(UrlTo.home());
            } catch(ex) {
              alert('Cannot delete the project');
            }
          }}
        />
      )}
    </ScreenLayout>
  );
};

export default OptionsPage;
