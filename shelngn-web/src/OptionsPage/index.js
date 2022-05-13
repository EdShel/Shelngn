import clsx from "clsx";
import React, { useCallback, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  deleteMember,
  deleteProject,
  deleteUnpublishProject,
  getProjectInfo,
  getScreenshotUrl,
  postAddMember,
  postPublishProject,
} from "../api";
import LineLoader from "../components/LineLoader";
import ScreenLayout, { contentClassName } from "../components/ScreenLayout";
import useAuth from "../hooks/useAuth";
import UrlTo from "../UrlTo";
import useWorkspaceId from "../WorkspacePage/hooks/useWorkspaceId";
import ConfirmModal from "./ConfirmModal";
import DeleteButton from "./DeleteButton";
import plusIcon from "../assets/plus.svg";
import { useShowAlertNotification } from "../InfoAlert";
import AddMemberModal from "./AddMemberModal";
import MemberCard from "./MemberCard";
import styles from "./styles.module.css";
import PublishButton from "./PublishButton";

const OptionsPage = () => {
  const workspaceId = useWorkspaceId();
  const [project, setProject] = useState(null);
  const [isLoading, setLoading] = useState(true);
  const [showDeleteProject, setShowDeleteProject] = useState(false);
  const [memberIdToDelete, setMemberIdToDelete] = useState(null);
  const [showAddMember, setShowAddMember] = useState(false);
  const { isLoading: isUserLoading, user } = useAuth();
  const [isPublishLoading, setPublishLoading] = useState(false);
  const navigate = useNavigate();
  const { showError, showInfo } = useShowAlertNotification();

  const fetchProjectInfo = useCallback(async () => {
    try {
      const projectResponse = await getProjectInfo(workspaceId);
      setProject(projectResponse);
    } catch (e) {
      showError("Error while loading the project information.");
    }
  }, [workspaceId, showError]);

  const handleTogglePublish = useCallback(async () => {
    setPublishLoading(true);
    try {
      if (!project.isPublished) {
        await postPublishProject(workspaceId);
      } else {
        await deleteUnpublishProject(workspaceId);
      }
      setProject((oldState) => ({ ...oldState, isPublished: !oldState.isPublished }));
    } catch (e) {
      const responseError = e.response?.data.error;
      showError(responseError || "Cannot change state, please try again later");
    }
    setPublishLoading(false);
  }, [project?.isPublished, showError, workspaceId]);

  useEffect(() => {
    const fetchProject = async () => {
      fetchProjectInfo();
      setLoading(false);
    };
    fetchProject();
  }, [fetchProjectInfo]);

  return (
    <ScreenLayout>
      <LineLoader isLoading={isLoading || isUserLoading} />
      <div className={clsx(contentClassName, styles.container)}>
        {project && (
          <>
            <div className={styles.header}>
              <h1 className={styles["header-text"]}>{project.projectName}</h1>
              <p>Created {new Date(project.insertDate).toLocaleString("en")}</p>
              <DeleteButton onPress={() => setShowDeleteProject(true)} />
              <PublishButton
                isPublished={project.isPublished}
                onTogglePublish={handleTogglePublish}
                isLoading={isPublishLoading}
              />
            </div>
            <h2>Members ({project.members.length})</h2>
            <div className={styles.members}>
              {project.members.map((member) => (
                <MemberCard
                  key={member.id}
                  userName={member.userName}
                  email={member.email}
                  role={member.memberRole}
                  isMe={member.id === user.id}
                  onRemove={() => setMemberIdToDelete(member.id)}
                />
              ))}
              <button className={styles["create-user-card"]} onClick={() => setShowAddMember(true)}>
                <img src={plusIcon} alt="Add" />
                Add new member
              </button>
            </div>
            <h2>Screenshots</h2>
            {project.screenshots.map((s) => (
              <img key={s.id} alt="Screenshot" src={getScreenshotUrl(workspaceId, s.imageUrl)} />
            ))}
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
            } catch (ex) {
              showError("Cannot delete the project");
            }
          }}
        />
      )}
      {memberIdToDelete && (
        <ConfirmModal
          title="Remove member"
          text="Do you really want to remove this member?"
          onOk={async () => {
            try {
              await deleteMember(workspaceId, memberIdToDelete);
              showInfo("Removed member");
              setMemberIdToDelete(null);
              await fetchProjectInfo();
            } catch (ex) {
              showError("Couldn't remove the member");
            }
          }}
          onCancel={() => setMemberIdToDelete(null)}
        />
      )}
      {showAddMember && (
        <AddMemberModal
          onAddMember={async ({ emailOrUserName }) => {
            try {
              await postAddMember(workspaceId, emailOrUserName);
              showInfo("Project member has been added");
              setShowAddMember(false);
              await fetchProjectInfo();
            } catch (e) {
              const errorText = e.response?.data.Message;
              showError(errorText || "Could not add project member");
            }
          }}
          onCancel={() => setShowAddMember(false)}
        />
      )}
    </ScreenLayout>
  );
};

export default OptionsPage;
