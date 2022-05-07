import React, { useEffect, useState } from "react";
import { getProjectInfo } from "../api";
import LineLoader from "../components/LineLoader";
import ScreenLayout, { contentClassName } from "../components/ScreenLayout";
import UserPreview from "../components/UserPreview";
import useWorkspaceId from "../WorkspacePage/hooks/useWorkspaceId";
import styles from "./styles.module.css";

const OptionsPage = () => {
  const workspaceId = useWorkspaceId();
  const [project, setProject] = useState(null);
  const [isLoading, setLoading] = useState(true);

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
      <div className={contentClassName}>
        <LineLoader isLoading={isLoading} />
        {JSON.stringify(project, 0, 2)}
        {project && (
          <>
            <h1>{project.projectName}</h1>
            <p>Created {new Date(project.insertDate).toLocaleString("en")}</p>
            <h2>Members ({project.members.length})</h2>
            <div className={styles.members}>
              {project.members.map((member) => (
                <div className={styles['user-card']}>
                  <UserPreview userName={member.userName} className={styles["user-avatar"]} />
                  <p>{member.email}</p>
                  <p>{member.userName}</p>
                </div>
              ))}
            </div>
          </>
        )}
      </div>
    </ScreenLayout>
  );
};

export default OptionsPage;
