import clsx from "clsx";
import React, { useCallback, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { deleteMember, deleteProject, getProjectInfo, postAddMember } from "../api";
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

const OptionsPage = () => {
  const workspaceId = useWorkspaceId();
  const [project, setProject] = useState(null);
  const [isLoading, setLoading] = useState(true);
  const [showDeleteProject, setShowDeleteProject] = useState(false);
  const [memberIdToDelete, setMemberIdToDelete] = useState(null);
  const [showAddMember, setShowAddMember] = useState(false);
  const { isLoading: isUserLoading, user } = useAuth();
  const navigate = useNavigate();
  const { showError, showInfo } = useShowAlertNotification();

  const fetchProjectInfo = useCallback(async () => {
    const projectResponse = await getProjectInfo(workspaceId);
    setProject(projectResponse);
  }, [workspaceId]);

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
